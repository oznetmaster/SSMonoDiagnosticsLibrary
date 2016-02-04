using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronLogger;
using System;

namespace SSMono.Diagnostics
	{
	public class CrestronLoggerTraceListener : TraceListener
		{
		private uint _debugLevel;
		private bool _logOnlyThisLevel;
		private LoggerModeEnum _loggerMode;

		private static object lockObject = new object ();

		public CrestronLoggerTraceListener ()
			: base ("CrestronLogger")
			{
			if (!CrestronLogger.LoggerInitialized)
				CrestronLogger.Initialize (1);
			}

		public CrestronLoggerTraceListener (uint paramDebugLevel)
			: this ()
			{
			_debugLevel = paramDebugLevel;
			}

		public CrestronLoggerTraceListener (uint paramDebugLevel, bool logOnlyThisLevel)
			: this (paramDebugLevel)
			{
			_logOnlyThisLevel = logOnlyThisLevel;
			}

		public CrestronLoggerTraceListener (uint paramDebugLevel, bool logOnlyThisLevel, LoggerModeEnum loggerMode)
			: this (paramDebugLevel, logOnlyThisLevel)
			{
			_loggerMode = loggerMode;
			}

		public CrestronLoggerTraceListener (string initializeData)
			{
			string[] info = initializeData.Split (new char[] {' ', ',', ';'});
			uint lev;
			bool logonly;

			if (info.Length == 0)
				return;
			if (info.Length > 0)
				{
				if (TryParsers.UInt32TryParse (info[0], out lev) && lev <= 10)
					_debugLevel = lev;

				if (info.Length > 1)
					{
					if (TryParsers.BooleanTryParse (info[1], out logonly))
						_logOnlyThisLevel = logonly;

					if (info.Length > 2)
						{
						LoggerModeEnum le;
						try
							{
							le = (LoggerModeEnum)Enum.Parse (typeof (LoggerModeEnum), info[2], true);
							_loggerMode = le;
							}
						catch (ArgumentException)
							{
							}
						}
					}
				}
			}

		private uint savedDebugLevel;
		private bool savedLogOnlyThisLevel;
		private LoggerModeEnum savedLoggerMode;
		private void SetState ()
			{
			CMonitor.Enter (lockObject);
			savedDebugLevel = CrestronLogger.DebugLevel;
			savedLogOnlyThisLevel = CrestronLogger.LogOnlyCurrentDebugLevel;
			savedLoggerMode = CrestronLogger.Mode;
			CrestronLogger.DebugLevel = _debugLevel;
			CrestronLogger.LogOnlyCurrentDebugLevel = _logOnlyThisLevel;
			CrestronLogger.Mode = _loggerMode;
			}

		private void RestoreState ()
			{
			CrestronLogger.DebugLevel = savedDebugLevel;
			CrestronLogger.LogOnlyCurrentDebugLevel = savedLogOnlyThisLevel;
			CrestronLogger.Mode = savedLoggerMode;
			CMonitor.Exit (lockObject);
			}

		public override void Write (string message)
			{
			SetState ();

			try
				{
				uint level = 0;
				if (message.StartsWith ("Error : 0 : "))
					level = 1;
				else if (message.StartsWith ("Warning : 0 : "))
					level = 4;
				else if (message.StartsWith ("Information : 0 : "))
					level = 10;

				CrestronLogger.WriteToLog (message, level);
				}
			finally
				{
				RestoreState ();
				}
			}

		public override void WriteLine (string message)
			{
			Write (message + CrestronEnvironment.NewLine);
			}

		public override void Flush ()
			{
			base.Flush ();

			SetState ();
			try
				{
				CrestronLogger.ForceFlush ();
				}
			finally
				{
				RestoreState ();
				}
			}
		}
	}