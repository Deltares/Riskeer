using System.Threading;
using DelftTools.Utils.Threading;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.Threading
{
    [TestFixture]
    public class ThreadsafeBindingListTest
    {
        //a synchronization context provides synchronized execution.
        //this one only checks the call to send was made.
        public class MockContext : SynchronizationContext
        {
            public int SendWasCalled { get; set; }
            //just run the thing.
            public override void Send(SendOrPostCallback d, object state)
            {
                SendWasCalled++;
            }
        }

        [Test]
        public void ListChangedEventIsAlwaysOnTheThreadThatCreatedTheList()
        {
            try
            {
                //convince the threadsafebindinglist we are using a synchronization context  
                //by providing a synchronization context
                var context = new MockContext();
                SynchronizationContext.SetSynchronizationContext(context);

                var list = new ThreadsafeBindingList<int>(context);

                //create a new thread adding something to the list
                //use an anonymous lambda expression delegate :)
                var addFiveThread = new Thread(() =>
                {
                    list.Add(5);
                });
                //action! run the thread adding 5
                addFiveThread.Start();
                //wait for the other thread to finish
                while (addFiveThread.IsAlive)
                    Thread.Sleep(10);

                //assert the list used the context.
                Assert.AreEqual(1, context.SendWasCalled, "The list did not use the send method the current context!");

            }
            finally
            {
                //always reset the synchronizationcontext
                SynchronizationContext.SetSynchronizationContext(null);
            }
            
        }
    }
}
