using System;
using System.Windows.Input;

namespace Server.Auxiliary.Commands
{

    /// <summary>
    /// The CommandEventHandler delegate.
    /// </summary>
    public delegate void CommandEventHandler(object sender, CommandEventArgs args);

    /// <summary>
    /// The typed CommandEventHandler delegate.
    /// </summary>
    public delegate void CommandEventHandler<TParameter>(object sender, CommandEventArgs<TParameter> args);

    /// <summary>
    /// The CancelCommandEvent delegate.
    /// </summary>
    public delegate void CancelCommandEventHandler(object sender, CancelCommandEventArgs args);

    /// <summary>
    /// The typed CancelCommandEvent delegate.
    /// </summary>
    public delegate void CancelCommandEventHandler<TParameter>(object sender, CancelCommandEventArgs<TParameter> args);


    /// <summary>
    /// The ViewModelCommand class - an ICommand that can fire a function.
    /// </summary>
    public class Command : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="canExecute">if set to <c>true</c> the command can execute.</param>
        public Command(Action action, bool canExecute = true)
        {
            //  Set the action.
            this.action = action;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="parameterizedAction">The parameterized action.</param>
        /// <param name="canExecute">if set to <c>true</c> the command can execute.</param>
        public Command(Action<object> parameterizedAction, bool canExecute = true)
        {
            //  Set the action.
            this.parameterizedAction = parameterizedAction;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Does the execute.
        /// </summary>
        public virtual void DoExecute()
        {
            //  Call the main command function.
            DoExecute(null);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="param">The param.</param>
        public virtual void DoExecute(object param)
        {
            //  Invoke the executing command, allowing the command to be cancelled.
            var args = new CancelCommandEventArgs { Parameter = param, Cancel = false };
            InvokeExecuting(args);

            //  If the event has been cancelled, bail now.
            if (args.Cancel)
                return;

            //  Call the action or the parameterized action, whichever has been set.
            param = args.Parameter;
            InvokeAction(param);

            //  Call the executed function.
            InvokeExecuted(new CommandEventArgs { Parameter = param });
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="param">The param.</param>
        protected void InvokeAction(object param)
        {
            var theAction = action;
            var theParameterizedAction = parameterizedAction;
            if (theAction != null)
                theAction();
            else if (theParameterizedAction != null)
                theParameterizedAction(param);
        }

        /// <summary>
        /// Invokes the executed event.
        /// </summary>
        /// <param name="args">The <see cref="Apex.MVVM.CommandEventArgs"/> instance containing the event data.</param>
        protected void InvokeExecuted(CommandEventArgs args)
        {
            //  Get the event.
            var theEvent = Executed;

            //  Call the executed event.
            if (theEvent != null)
                theEvent(this, args);
        }

        /// <summary>
        /// Invokes the executing event.
        /// </summary>
        /// <param name="args">The <see cref="Apex.MVVM.CancelCommandEventArgs"/> instance containing the event data.</param>
        protected void InvokeExecuting(CancelCommandEventArgs args)
        {
            //  Get the event.
            var theEvent = Executing;

            //  Call the executed event.
            if (theEvent != null)
                theEvent(this, args);
        }

        /// <summary>
        /// The action that will be called when the command is invoked.
        /// </summary>
        protected Action action;

        /// <summary>
        /// The parameterised action that will be called when the command is invoked.
        /// </summary>
        protected Action<object> parameterizedAction;

        /// <summary>
        /// Bool indicating whether the command can execute.
        /// </summary>
        private bool canExecute;

        /// <summary>
        /// Gets or sets a value indicating whether this instance can execute.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can execute; otherwise, <c>false</c>.
        /// </value>
        public bool CanExecute
        {
            get { return canExecute; }
            set
            {
                if (canExecute == value)
                    return;
                canExecute = value;

                var theEvent = CanExecuteChanged;
                if (theEvent != null)
                    theEvent(this, EventArgs.Empty);
            }
        }

        #region ICommand Members

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        bool ICommand.CanExecute(object parameter)
        {
            return canExecute;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        void ICommand.Execute(object parameter)
        {
            DoExecute(parameter);
        }

        #endregion

        /// <summary>
        /// Occurs when can execute is changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Occurs when the command is about to execute.
        /// </summary>
        public event CancelCommandEventHandler Executing;

        /// <summary>
        /// Occurs when the command executed.
        /// </summary>
        public event CommandEventHandler Executed;
    }

    /// <summary>
    /// The Command class - an ICommand that can fire a function.
    /// This version of the Command class invokes a function that takes TParameter as its parameter.
    /// </summary>
    public class Command<TParameter> : ICommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="parameterizedAction">The parameterized action.</param>
        /// <param name="canExecute">if set to <c>true</c> the command can execute.</param>
        public Command(Action<TParameter> parameterizedAction, bool canExecute = true)
        {
            //  Set the action.
            this.parameterizedAction = parameterizedAction;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="param">The param.</param>
        public virtual void DoExecute(TParameter param)
        {
            //  Invoke the executing command, allowing the command to be cancelled.
            var args = new CancelCommandEventArgs<TParameter> { Parameter = param, Cancel = false };
            InvokeExecuting(args);

            //  If the event has been cancelled, bail now.
            if (args.Cancel)
                return;

            //  Call the action or the parameterized action, whichever has been set.
            param = args.Parameter;
            InvokeAction(param);

            //  Call the executed function.
            InvokeExecuted(new CommandEventArgs<TParameter> { Parameter = param });
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="param">The param.</param>
        protected void InvokeAction(TParameter param)
        {
            var theParameterizedAction = parameterizedAction;
            if (theParameterizedAction != null)
                theParameterizedAction(param);
        }

        /// <summary>
        /// Invokes the executed event.
        /// </summary>
        /// <param name="args">The <see cref="Apex.MVVM.CommandEventArgs"/> instance containing the event data.</param>
        protected void InvokeExecuted(CommandEventArgs<TParameter> args)
        {
            //  Get the event.
            var theEvent = Executed;

            //  Call the executed event.
            if (theEvent != null)
                theEvent(this, args);
        }

        /// <summary>
        /// Invokes the executing event.
        /// </summary>
        /// <param name="args">The <see cref="Apex.MVVM.CancelCommandEventArgs"/> instance containing the event data.</param>
        protected void InvokeExecuting(CancelCommandEventArgs<TParameter> args)
        {
            //  Get the event.
            var theEvent = Executing;

            //  Call the executed event.
            if (theEvent != null)
                theEvent(this, args);
        }

        /// <summary>
        /// The parameterised action that will be called when the command is invoked.
        /// </summary>
        protected Action<TParameter> parameterizedAction;

        /// <summary>
        /// Bool indicating whether the command can execute.
        /// </summary>
        private bool canExecute;

        /// <summary>
        /// Gets or sets a value indicating whether this instance can execute.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can execute; otherwise, <c>false</c>.
        /// </value>
        public bool CanExecute
        {
            get { return canExecute; }
            set
            {
                if (canExecute == value)
                    return;
                canExecute = value;

                var theEvent = CanExecuteChanged;
                if (theEvent != null)
                    theEvent(this, EventArgs.Empty);
            }
        }

        #region ICommand Members

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        bool ICommand.CanExecute(object parameter)
        {
            return canExecute;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        void ICommand.Execute(object parameter)
        {
            //  This is the place where we can have problems - if the 
            //  user has bound to the wrong data type.
            if (parameter is TParameter == false)
            {
                var passedType = parameter != null ? parameter.GetType() : typeof(object);
                throw new InvalidOperationException("A parameter of type " + passedType.Name +
                                                    " was passed to a Command expecting " +
                                                    "a parameter of type " + typeof(TParameter).Name +
                                                    ". Check the binding of the 'CommandParameter'.");
            }

            //  Execute the command.
            DoExecute((TParameter)parameter);
        }

        #endregion

        /// <summary>
        /// Occurs when can execute is changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Occurs when the command is about to execute.
        /// </summary>
        public event CancelCommandEventHandler<TParameter> Executing;

        /// <summary>
        /// Occurs when the command executed.
        /// </summary>
        public event CommandEventHandler<TParameter> Executed;
    }

    /// <summary>
    /// CancelCommandEventArgs is sent during the command process, just
    /// before execution. It supplies the command parameter and can
    /// indicate that the command should be cancelled.
    /// </summary>
    public class CancelCommandEventArgs : CommandEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CancelCommandEventArgs"/> command should be cancelled.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; }
    }

    /// <summary>
    /// Typed CancelCommandEventArgs is sent during the command process, just
    /// before execution. It supplies the command parameter and can
    /// indicate that the command should be cancelled.
    /// </summary>
    public class CancelCommandEventArgs<TParameter> : CommandEventArgs<TParameter>
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CancelCommandEventArgs"/> command should be cancelled.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; }
    }

    /// <summary>
    /// CommandEventArgs, event arguments for a command event, which also stores the command
    /// parameter.
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>The parameter.</value>
        public object Parameter { get; set; }
    }

    /// <summary>
    /// Typed CommandEventArgs, event arguments for a command event, which also stores the command
    /// parameter.
    /// </summary>
    public class CommandEventArgs<TParameter> : EventArgs
    {
        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>The parameter.</value>
        public TParameter Parameter { get; set; }
    }
}
