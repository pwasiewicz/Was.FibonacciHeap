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

            Assert.AreEqual("b", heap.PopMin().Value);
            Assert.AreEqual("a", heap.PopMin().Value);
        }

        [TestMethod]
        public void DecreaseKey_SampleInts_RetunrsProperMin()
        {
            var heap = new FibonacciHeap<string, int>();

            var aNode = heap.Push("a", 20);
            heap.Push("b", 10);

            heap.DecreaseKey(aNode, 5);

            Assert.AreEqual("a", heap.PopMin().Value);
            Assert.AreEqual("b", heap.PopMin().Value);
        }


        [TestMethod]
        public void Pop_SampleInts_ReturnsProperMin()
        {
            var heap = new FibonacciHeap<string, int>();

            heap.Push("a", 20);
            heap.Push("b", 10);

            heap.PopMin();
            
            Assert.AreEqual("a", heap.PopMin().Value);
        }
    }
}
