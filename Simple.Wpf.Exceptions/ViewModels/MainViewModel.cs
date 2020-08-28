namespace Simple.Wpf.Exceptions.ViewModels
{
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Commands;
    using Extensions;
    using Services;

    public sealed class MainViewModel : BaseViewModel, IMainViewModel
    {
        public MainViewModel(ISchedulerService schedulerService)
        {
            ThrowFromUiThreadCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            ThrowFromTaskCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            ThrowFromRxCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            void ThrowFromUiThreadCommandExecute(object x)
            {
                Logger.Info("ThrowFromUiThreadCommand executing...");
                throw new Exception(x + " - thrown from UI thread.");
            }

            ThrowFromUiThreadCommand
                .ActivateGestures()
                .SafeSubscribe(ThrowFromUiThreadCommandExecute, schedulerService.Dispatcher)
                .DisposeWith(this);

            void ThrowFromTaskCommandExecute(object x)
            {
                Logger.Info("ThrowFromTaskCommand executing...");

                void ThrownFromTaskAction()
                {
                    Thread.Sleep(1000);

                    throw new Exception(x + " - thrown from Task StartNew.");
                }

                Task.Factory.StartNew((Action)ThrownFromTaskAction, TaskCreationOptions.LongRunning);
            }

            ThrowFromTaskCommand
                .ActivateGestures()
                .Subscribe((Action<object>)ThrowFromTaskCommandExecute)
                .DisposeWith(this);

            void ThrowFromRxCommandExecute(object x)
            {
                Logger.Info("ThrowFromRxCommand executing...");

                void ThrownFromRxAction()
                {
                    Thread.Sleep(1000);

                    throw new Exception(x + " - thrown from Rx Start.");
                }

                Observable.Start(ThrownFromRxAction, schedulerService.TaskPool)
                    .Take(1)
                    .Subscribe();
            }

            ThrowFromRxCommand
                .ActivateGestures()
                .Subscribe((Action<object>)ThrowFromRxCommandExecute)
                .DisposeWith(this);
        }

        public ReactiveCommand<object> ThrowFromUiThreadCommand { get; }

        public ReactiveCommand<object> ThrowFromTaskCommand { get; }

        public ReactiveCommand<object> ThrowFromRxCommand { get; }
    }
}