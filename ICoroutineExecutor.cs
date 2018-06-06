using System.Collections;

namespace OpenAPE
{
    /// <summary>
    /// Implement this interface to offer your children a way to call a coroutine without being a MonoBehavior.
    /// </summary>
    public interface ICoroutineExecutor
    {
        /// <summary>
        /// This method provides a convenient way for the child to run a coroutine.
        /// </summary>
        /// <param name="coroutineMethod">The method to execute.</param>
        void StartChildCoroutine(IEnumerator coroutineMethod);
    }
}