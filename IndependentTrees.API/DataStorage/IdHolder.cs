namespace IndependentTrees.API.DataStorage
{
    public class IdHolder
    {
        private int _id;

        public void SetID(int id)
        {
            _id = id;
        }

        public int GetNextID() => Interlocked.Increment(ref _id);
    }
}
