using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    public class cycleCommonClass
    {
        private int currentCycleTimereq;
        private DateTime cyclestartTime;
        private int startTimeHronly;
        private int startTimeMinonly;
        private int startTimeSeconly;
        private string ovencondOnOff = "OFF";
        private int endTempOfCycle;
        private int startTempOfCycle;
        private string finishTimeShow;
        private string cycleInd;

        public int CurrentCycleTimereq
        {
            get
            {
                return currentCycleTimereq;
            }

            set
            {
                currentCycleTimereq = value;
            }
        }

        public DateTime CyclestartTime
        {
            get
            {
                return cyclestartTime;
            }

            set
            {
                cyclestartTime = value;
            }
        }

        public int StartTimeHronly
        {
            get
            {
                return startTimeHronly;
            }

            set
            {
                startTimeHronly = value;
            }
        }

        public int StartTimeMinonly
        {
            get
            {
                return startTimeMinonly;
            }

            set
            {
                startTimeMinonly = value;
            }
        }

        

        public int StartTimeSeconly
        {
            get
            {
                return startTimeSeconly;
            }

            set
            {
                startTimeSeconly = value;
            }
        }

        public string OvencondOnOff
        {
            get
            {
                return ovencondOnOff;
            }

            set
            {
                ovencondOnOff = value;
            }
        }

        public int EndTempOfCycle
        {
            get
            {
                return endTempOfCycle;
            }

            set
            {
                endTempOfCycle = value;
            }
        }

        public int StartTempOfCycle
        {
            get
            {
                return startTempOfCycle;
            }

            set
            {
                startTempOfCycle = value;
            }
        }

        public string FinishTimeShow
        {
            get
            {
                return finishTimeShow;
            }

            set
            {
                finishTimeShow = value;
            }
        }

        public string CycleInd
        {
            get
            {
                return cycleInd;
            }

            set
            {
                cycleInd = value;
            }
        }
    }
}
