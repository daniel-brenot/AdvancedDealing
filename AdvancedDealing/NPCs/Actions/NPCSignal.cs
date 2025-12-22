namespace AdvancedDealing.NPCs.Actions
{
    public class NPCSignal : NPCAction
    {
        protected override string ActionType =>
            "NPCSignal";

        public bool StartedThisCycle { get; protected set; }

        public override void Start()
        {
            base.Start();

            StartedThisCycle = true;
        }

        public override void End()
        {
            base.End();

            StartedThisCycle = false;
        }

        public override void Interrupt()
        {
            StartedThisCycle = false;

            base.Interrupt();
        }

        public override void MinPassed()
        {
            base.MinPassed();

            if (StartedThisCycle && !ShouldStart())
            {
                StartedThisCycle = false;
            }
        }

        public override bool ShouldStart()
        {
            if (StartedThisCycle)
            {
                return false;
            }

            return base.ShouldStart();
        }
    }
}
