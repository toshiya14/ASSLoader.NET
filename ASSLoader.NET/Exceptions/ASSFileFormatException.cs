using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ASSLoader.NET.Exceptions
{
    public class ASSFileFormatException:Exception
    {
        private int Line { get; set; }

        private string FileName { get; set; }

        public new string Message { get; private set; }

        public override IDictionary Data {
            get {
                return new Dictionary<string, object> {
                    { "Line", Line },
                    { "FileName", FileName },
                    { "Message", Message }
                };
            }
        }

        public ASSFileFormatException(string filename, int linenum, string message)
        {
            this.Line = linenum;
            this.FileName = filename;
            this.Message = message;
        }
    }
}
