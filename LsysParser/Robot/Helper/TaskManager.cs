using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LsysParser.Robot.Helper
{
    public class TaskManager
    {
        int threadsCount;
        bool isParallel;

        public int ThreadsCount
        {
            get
            {
                return threadsCount;
            }

            set
            {
                threadsCount = value;
            }
        }

        public TaskManager(int threadsCount, bool isParallel)
        {
            this.threadsCount = threadsCount;
            this.isParallel = isParallel;
        }

        int working;

        public void NewTask(Action func)
        {
            if (isParallel)
            {
                while (working >= threadsCount)
                    Thread.Sleep(100);

                Interlocked.Increment(ref working);
                Task.Run(() =>
                {
                    try
                    {
                        func();
                    }
                    finally
                    {
                        Interlocked.Decrement(ref working);
                    }
                });
            }
            else
                func();
        }

        public void NewTask<T1, T2>(Action<T1, T2> func, T1 arg, T2 arg2)
        {
            if (isParallel)
            {
                while (working >= threadsCount)
                    Thread.Sleep(100);

                Interlocked.Increment(ref working);
                Task.Run(() =>
                {
                    try
                    {
                        func(arg, arg2);
                    }
                    finally
                    {
                        Interlocked.Decrement(ref working);
                    }
                });
            }
            else
                func(arg, arg2);
        }

        public void NewTask<T1>(Action<T1> func, T1 arg)
        {
            if (isParallel)
            {
                while (working >= threadsCount)
                    Thread.Sleep(100);

                Interlocked.Increment(ref working);
                Task.Run(() =>
                {
                    try
                    {
                        func(arg);
                    }
                    finally
                    {
                        Interlocked.Decrement(ref working);
                    }
                });
            }
            else
                func(arg);
        }

        public void WaitAllTasks()
        {
            while (working > 0)
                Thread.Sleep(100);
        }
    }
}
