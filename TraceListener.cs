//
// System.Diagnostics.TraceListener.cs
//
// Authors:
//   Jonathan Pryor (jonpryor@vt.edu)
//   Atsushi Enomoto (atsushi@ximian.com)
//
// Comments from John R. Hicks <angryjohn69@nc.rr.com> original implementation 
// can be found at: /mcs/docs/apidocs/xml/en/System.Diagnostics
//
// (C) 2002 Jonathan Pryor
// (C) 2007 Novell, Inc.
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SSMono.Diagnostics
	{
	public abstract class TraceListener : IDisposable
		{
		private StringDictionary attributes = new StringDictionary ();

		protected TraceListener ()
			: this ("")
			{
			}

		protected TraceListener (string name)
			{
			IndentLevel = 0;
			IndentSize = 4;
			NeedIndent = true;
			Name = name;
			}

		public int IndentLevel { get; set; }

		public int IndentSize { get; set; }

		public virtual string Name { get; set; }

		protected bool NeedIndent { get; set; }

		public virtual bool IsThreadSafe
			{
			get { return false; }
			}

		public virtual void Close ()
			{
			Dispose ();
			}

		public void Dispose ()
			{
			Dispose (true);
			//GC.SuppressFinalize (this);
			}

		protected virtual void Dispose (bool disposing)
			{
			}

		public virtual void Fail (string message)
			{
			Fail (message, null);
			}

		public virtual void Fail (string message, string detailMessage)
			{
			WriteLine ("---- DEBUG ASSERTION FAILED ----");
			WriteLine ("---- Assert Short Message ----");
			WriteLine (message);
			if (detailMessage != null)
				{
				WriteLine ("---- Assert Long Message ----");
				WriteLine (detailMessage);
				}

			WriteLine ("");
			}

		public virtual void Flush ()
			{
			}

		public virtual void Write (object o)
			{
			Write (o.ToString ());
			}

		public abstract void Write (string message);

		public virtual void Write (object o, string category)
			{
			Write (o.ToString (), category);
			}

		public virtual void Write (string message, string category)
			{
			Write (category + ": " + message);
			}

		protected virtual void WriteIndent ()
			{
			// Must set NeedIndent to false before Write; otherwise, we get endless
			// recursion with Write->WriteIndent->Write->WriteIndent...*boom*
			NeedIndent = false;
			var indent = new String (' ', IndentLevel * IndentSize);
			Write (indent);
			}

		public virtual void WriteLine (object o)
			{
			WriteLine (o.ToString ());
			}

		public abstract void WriteLine (string message);

		public virtual void WriteLine (object o, string category)
			{
			WriteLine (o.ToString (), category);
			}

		public virtual void WriteLine (string message, string category)
			{
			WriteLine (category + ": " + message);
			}

		internal static string FormatArray (ICollection list, string joiner)
			{
			var arr = new string[list.Count];
			int i = 0;
			foreach (object o in list)
				arr[i++] = o != null ? o.ToString () : String.Empty;
			return String.Join (joiner, arr);
			}

		public virtual void TraceData (TraceEventCache eventCache, string source,
			TraceEventType eventType, int id, object data)
			{
			if (Filter != null &&
				!Filter.ShouldTrace (eventCache, source, eventType,
						 id, null, null, data, null))
				return;

			WriteLine (String.Format ("{0} {1}: {2} : {3}", source, eventType, id, data));

			if (eventCache == null)
				return;

			if ((TraceOutputOptions & TraceOptions.ProcessId) != 0)
				WriteLine ("    ProcessId=" + eventCache.ProcessId);
#if !NETCF
			if ((TraceOutputOptions & TraceOptions.LogicalOperationStack) != 0)
				WriteLine ("    LogicalOperationStack=" + FormatArray (eventCache.LogicalOperationStack, ", "));
#endif
			if ((TraceOutputOptions & TraceOptions.ThreadId) != 0)
				WriteLine ("    ThreadId=" + eventCache.ThreadId);
			if ((TraceOutputOptions & TraceOptions.DateTime) != 0)
				WriteLine ("    DateTime=" + eventCache.DateTime.ToString ("o"));
			if ((TraceOutputOptions & TraceOptions.Timestamp) != 0)
				WriteLine ("    Timestamp=" + eventCache.Timestamp);
			if ((TraceOutputOptions & TraceOptions.Callstack) != 0)
				WriteLine ("    Callstack=" + eventCache.Callstack);
			}

		public virtual void TraceData (TraceEventCache eventCache, string source,
			TraceEventType eventType, int id, params object[] data)
			{
			if (Filter != null &&
				!Filter.ShouldTrace (eventCache, source, eventType,
			    		 id, null, null, null, data))
				return;

			TraceData (eventCache, source, eventType, id, FormatArray (data, " "));
			}

		public virtual void TraceEvent (TraceEventCache eventCache, string source, TraceEventType eventType, int id)
			{
			TraceEvent (eventCache, source, eventType, id, null);
			}

		public virtual void TraceEvent (TraceEventCache eventCache, string source, TraceEventType eventType,
			int id, string message)
			{
			TraceData (eventCache, source, eventType, id, message);
			}

		public virtual void TraceEvent (TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
			{
			TraceEvent (eventCache, source, eventType, id, String.Format (format, args));
			}

		public virtual void TraceTransfer (TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
			{
			TraceEvent (eventCache, source, TraceEventType.Transfer, id, String.Format ("{0}, relatedActivityId={1}", message, relatedActivityId));
			}

		protected internal virtual string[] GetSupportedAttributes ()
			{
			return null;
			}

		public StringDictionary Attributes
			{
			get { return attributes; }
			}

		public TraceFilter Filter { get; set; }

		public TraceOptions TraceOutputOptions { get; set; }
		}
	}

