using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TaskbarMonitor.Counters
{
    class CounterMemory: ICounter
    {
        public CounterMemory(Options options)
           : base(options)
        {}
      
        ulong totalMemory = 0;
        //Dictionary<CounterType, List<CounterInfo>> info = new Dictionary<CounterType, List<CounterInfo>>();

        public override void Initialize()
        {
            totalMemory = new ComputerInfo().TotalPhysicalMemory;
            ulong totalMemoryGB = totalMemory / 1024 / 1024 / 1024; // bytes -> GB

            lock (ThreadLock)
            {
                InfoSummary = new CounterInfo() { Name = "summary", History = new List<float>(), MaximumValue = totalMemoryGB };
                Infos = new List<CounterInfo>();
                Infos.Add(new CounterInfo() { Name = "U", History = new List<float>(), MaximumValue = totalMemoryGB });
            }

            /*
            info.Add(CounterType.SINGLE, new List<CounterInfo> {
                new CounterInfo() { Name = "default", History = new List<float>(), MaximumValue = totalMemory / 1024 }
            });*/
        }
        public override void Update()
        {
            float currentValue = totalMemory - new ComputerInfo().AvailablePhysicalMemory;
            currentValue = currentValue / 1024 / 1024 / 1024; // bytes -> GB

            lock (ThreadLock)
            {
                InfoSummary.CurrentValue = currentValue;
                InfoSummary.History.Add(currentValue);
                if (InfoSummary.History.Count > Options.HistorySize) InfoSummary.History.RemoveAt(0);

                InfoSummary.CurrentStringValue = (InfoSummary.CurrentValue).ToString("0.0") + "GB";

                {
                    var info = Infos.Where(x => x.Name == "U").Single();
                    info.CurrentValue = currentValue;
                    info.History.Add(currentValue);
                    if (info.History.Count > Options.HistorySize) info.History.RemoveAt(0);

                    info.CurrentStringValue = (info.CurrentValue).ToString("0.0") + "GB";
                }

            }
            

        }
       

        public override string GetName()
        {
            return "MEM";
        }

        public override CounterType GetCounterType()
        {
            return CounterType.SINGLE;
        }
    }
}
