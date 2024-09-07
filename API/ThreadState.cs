using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace hid_sharp.API;

public struct ThreadBarrier(int tripCount) {
    private readonly Mutex mutex = new();
    private readonly ManualResetEvent condition = new(false);
    public int Count { get; private set; } = 0;
    public int TripCount { get; private set; } = tripCount;

    public void SignalAndWait() {
        mutex.WaitOne();

        Count++;
        if (Count >= TripCount) {
            condition.Set();
        }

        mutex.ReleaseMutex();
        condition.WaitOne();
    }
}

/*
public struct ThreadState(int barrierTripCount) {
    public Thread? Thread { get; set; }
    public Mutex Mutex { get; set; } = new Mutex(); // Protects input_reports
    public AutoResetEvent Condition { get; set; } = new AutoResetEvent(false);
    public ThreadBarrier Barrier { get; set; } = new ThreadBarrier(barrierTripCount);
}*/

[StructLayout(LayoutKind.Sequential)]
public struct ThreadState {
    public nint thread; // HANDLE for thread on Windows
    public nint mutex;  // HANDLE for mutex
    public nint condition;  // HANDLE for event or condition variable
    public nint barrier; // Use a custom barrier implementation or other synchronization primitive
}
