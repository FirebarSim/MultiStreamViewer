using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MultiStreamViewer.Services
{
    public class FfmpegFrameService : IDisposable
    {
        private Process? _proc;
        private CancellationTokenSource? _cts;
        private readonly string _source;

        public event Action<byte[]>? FrameReady;

        public FfmpegFrameService(string source)
        {
            _source = source;
        }

        public bool IsRunning => _proc != null && !_proc.HasExited;

        public void Start()
        {
            if (IsRunning)
                return;

            _cts = new CancellationTokenSource();
            StartProcess(_cts.Token);
        }

        private void StartProcess(CancellationToken ct)
        {
            var psi = new ProcessStartInfo("ffmpeg")
            {
                Arguments = $"-rtsp_transport tcp -i \"{_source}\" -an -c:v mjpeg -f image2pipe -",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _proc = Process.Start(psi);
            if (_proc == null)
                return;

            _ = Task.Run(() => ReadLoop(_proc.StandardOutput.BaseStream, ct));
            _ = Task.Run(() => ReadErrors(_proc.StandardError));
        }

        private async Task ReadErrors(StreamReader sr)
        {
            try
            {
                while (!sr.EndOfStream)
                {
                    var _ = await sr.ReadLineAsync();
                }
            }
            catch { }
        }

        private async Task ReadLoop(Stream stream, CancellationToken ct)
        {
            var buffer = new List<byte>();
            var readBuf = new byte[4096];

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    int read = await stream.ReadAsync(readBuf.AsMemory(0, readBuf.Length), ct);
                    if (read <= 0)
                    {
                        await Task.Delay(50, ct);
                        continue;
                    }

                    lock (buffer)
                    {
                        for (int i = 0; i < read; i++)
                            buffer.Add(readBuf[i]);
                    }

                    // try extract frames
                    bool found = true;
                    while (found)
                    {
                        found = false;
                        int soi = -1, eoi = -1;
                        lock (buffer)
                        {
                            for (int i = 0; i + 1 < buffer.Count; i++)
                            {
                                if (buffer[i] == 0xFF && buffer[i + 1] == 0xD8)
                                {
                                    soi = i;
                                    break;
                                }
                            }

                            if (soi == -1)
                                break;

                            for (int j = soi + 2; j + 1 < buffer.Count; j++)
                            {
                                if (buffer[j] == 0xFF && buffer[j + 1] == 0xD9)
                                {
                                    eoi = j + 1;
                                    break;
                                }
                            }

                            if (eoi == -1)
                                break;

                            var frame = buffer.GetRange(soi, eoi - soi + 1).ToArray();
                            // remove up to eoi
                            buffer.RemoveRange(0, eoi + 1);
                            found = true;

                            try
                            {
                                FrameReady?.Invoke(frame);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
        }

        public void Stop()
        {
            try
            {
                _cts?.Cancel();
            }
            catch { }

            try
            {
                if (_proc != null && !_proc.HasExited)
                {
                    _proc.Kill(true);
                }
            }
            catch { }

            _proc = null;
        }

        public void Dispose()
        {
            Stop();
            _cts?.Dispose();
        }
    }
}
