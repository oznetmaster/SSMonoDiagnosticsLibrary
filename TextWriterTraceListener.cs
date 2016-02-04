//
// System.Diagnostics.TextWriterTraceListener.cs
//
// Comments from John R. Hicks <angryjohn69@nc.rr.com> original implementation 
// can be found at: /mcs/docs/apidocs/xml/en/System.Diagnostics
//
// Authors:
//   Jonathan Pryor (jonpryor@vt.edu)
//
// (C) 2002 Jonathan Pryor
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
using Crestron.SimplSharp.CrestronIO;
using MTextWriter = SSMono.IO.TextWriter;
using MStreamWriter = SSMono.IO.StreamWriter;
using Path = SSMono.IO.Path;

namespace SSMono.Diagnostics
	{
	public class TextWriterTraceListener : TraceListener
		{

		private TextWriter writer;
		private MTextWriter mwriter;

		public TextWriterTraceListener ()
			: base ("TextWriter")
			{
			}

		public TextWriterTraceListener (Stream stream)
			: this (stream, "")
			{
			}

		public TextWriterTraceListener (string fileName)
			: this (fileName, "")
			{
			}

		public TextWriterTraceListener (TextWriter writer)
			: this (writer, "")
			{
			}

		public TextWriterTraceListener (MTextWriter mwriter)
			: this (mwriter, "")
			{
			}

		public TextWriterTraceListener (Stream stream, string name)
			: base (name ?? "")
			{
			if (stream == null)
				throw new ArgumentNullException ("stream");
			if (stream.GetType () == typeof (Stream))
				writer = new StreamWriter (stream);
			else
				mwriter = new MStreamWriter (stream);
			}

		public TextWriterTraceListener (string fileName, string name)
			: base (name ?? "")
			{
			if (fileName == null)
				throw new ArgumentNullException ("fileName");
			writer = new StreamWriter (new FileStream (Path.GetFullPath (fileName), FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
			}

		public TextWriterTraceListener (TextWriter writer, string name)
			: base (name ?? "")
			{
			if (writer == null)
				throw new ArgumentNullException ("writer");
			this.writer = writer;
			}

		public TextWriterTraceListener (MTextWriter mwriter, string name)
			: base (name ?? "")
			{
			if (mwriter == null)
				throw new ArgumentNullException ("writer");
			this.mwriter = mwriter;
			}

		public TextWriter Writer
			{
			get { return writer; }
			set
				{
				writer = value;
				mwriter = null;
				}
			}

		public MTextWriter MonoWriter
			{
			get { return mwriter; }
			set
				{
				mwriter = value;
				writer = null;
				}
			}

		public override void Close ()
			{
			if (writer != null)
				{
				writer.Flush ();
				writer.Close ();
				writer = null;
				}
			else if (mwriter != null)
				{
				mwriter.Flush ();
				mwriter.Close ();
				mwriter = null;
				}
			}

		protected override void Dispose (bool disposing)
			{
			if (disposing)
				Close ();

			base.Dispose (disposing);
			}

		public override void Flush ()
			{
			if (writer != null)
				writer.Flush ();
			else if (mwriter != null)
				mwriter.Flush ();
			}

		public override void Write (string message)
			{
			if (writer != null)
				{
				if (NeedIndent)
					WriteIndent ();
				writer.Write (message);
				}
			else if (mwriter != null)
				{
				if (NeedIndent)
					WriteIndent ();
				mwriter.Write (message);
				}
			}

		public override void WriteLine (string message)
			{
			if (writer != null)
				{
				if (NeedIndent)
					WriteIndent ();
				writer.WriteLine (message);
				NeedIndent = true;
				}
			else if (mwriter != null)
				{
				if (NeedIndent)
					WriteIndent ();
				mwriter.WriteLine (message);
				NeedIndent = true;
				}
			}
		}
	}

