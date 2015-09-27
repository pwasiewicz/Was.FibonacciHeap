namespace Was.FibonacciHeapTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FibonacciHeap;

    [TestClass]
    public class FibonacciHeapTests
    {
        [TestMethod]
        public void Push_SampleInts_RetunrsProperMin()
        {
            var heap = new FibonacciHeap<string, int>();

            heap.Push("a", 2);
            heap.Push("b", 1);

            Assert.AreEqual("b", heap.Pop().Value);
            Assert.AreEqual("a", heap.Pop().Value);
        }

        [TestMethod]
        public void DecreaseKey_SampleInts_RetunrsProperMin()
        {
            var heap = new FibonacciHeap<string, int>();

            var aNode = heap.Push("a", 20);
            heap.Push("b", 10);

            heap.DecreaseKey(aNode, 5);

            Assert.AreEqual("a", heap.Pop().Value);
            Assert.AreEqual("b", heap.Pop().Value);
        }


        [TestMethod]
        public void Pop_SampleInts_ReturnsProperMin()
        {
            var heap = new FibonacciHeap<string, int>();

            heap.Push("a", 20);
            heap.Push("b", 10);

            heap.Pop();
            
            Assert.AreEqual("a", heap.Pop().Value);
        }

        [TestMethod, ExpectedException(typeof (EmptyHeapException))]
        public void Min_WhenEmptyHeap_ThrowsException()
        {
            var heap = new FibonacciHeap<string, int>();
            // ReSharper disable once UnusedVariable
            var min = heap.Min;
        }
    }
}
