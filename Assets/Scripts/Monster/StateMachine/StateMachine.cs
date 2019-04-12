namespace StateSystem
{
    public class StateMachine<T>
    {
        public State<T> currentState { get; private set; }
        public T owner;

        public StateMachine(T owner)
        {
            this.owner = owner;
            currentState = null;
        }

        public void ChangeState(State<T> newState)
        {
            if (currentState != null)
            {
                currentState.ExitState();
            }

            currentState = newState;
            currentState.EnterState();
        }

        public void Update()
        {
            currentState.ExecuteState();
        }
    }

    public abstract class State<T>
    {
        protected T owner;

        public abstract void EnterState();
        public abstract void ExecuteState();
        public abstract void ExitState();

        public State(T owner)
        {
            this.owner = owner;
        }
    }
}
