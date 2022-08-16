namespace BulletNET.Services.Devices.Base
{
    public abstract class Test : ITest
    {
        public bool IsScheduled { get; protected set; }
        public string TestName { get; protected set; }
        public DateTime TimeStamp { get; protected set; }
        public bool IsPassed { get; protected set; }
        public double Measured { get; protected set; }

        public bool IsRunning { get; protected set; }
        public bool IsError { get; protected set; }
        public bool isEnabled { get; set; }

        

        protected virtual void StartTest(string testName)
        {
            TestName = testName;
            IsRunning = true;
            IsError = false;
        }

        protected virtual void EndTest()
        {
            TimeStamp = DateTime.Now;
            IsScheduled = false;
            IsRunning = false;
        }
    }
}