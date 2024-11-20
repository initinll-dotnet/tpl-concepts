using AsyncAwait;
using ConcurrentCollections;
using DataSharingAndSynchronization;
using ParallelLinq;
using ParallelLoops;
using TaskCoordination;
using TaskProgramming;

//// 1.TaskProgramming Concepts
// IntroducingTasks.Execute();
// CancelingTasks.Execute();
// WaitingForTimeToPass.Execute();
// WaitingForTasks.Execute();
// ExceptionHandling.Execute();

//// 2.DataSharingAndSynchronization Concepts
// CriticalSections.Execute();
// InterlockedOperations.Execute();
// SpinLocking.Execute();
// MutexExample.Execute();
// ReaderWriterLocks.Execute();

//// 3.ConcurrentCollections
// ConcurrentDictionaryDemo.Execute();
// ConcurrentQueueDemo.Execute();
// ConcurrentStackDemo.Execute();
// ConcurrentBagDemo.Execute();
// BlockingCollectionDemo.Execute();
// ChannelDemo.Execute();

//// 4.TaskCoordination
// CreatingContinuationsDemo.Execute();
// ChildTasksDemo.Execute();
// BarrierDemo.Execute();
// CountDownEventDemo.Execute();
// ResetEventsDemo.Execute();
// SemaphoreSlimDemo.Execute();

//// 5.ParallelLoops
// ParallelLoopsDemo.Execute();
// BreakingAndStoppingDemo.Execute();
// ThreadLocalStorageDemo.Execute();
// PartitioningDemo.Execute();

//// 6.ParallelLinq
// AsParallelExample.Execute();
// CancellationAndExceptions.Execute();
// MergeOptions.Execute();
// CustomAggregationDemo.Execute();

//// 7.AsyncAwait
var asyncAwait = new AsyncAwaitDemo();
// asyncAwait.Execute1();
await asyncAwait.Execute2();
// await AsyncFactoryMethod.Execute();