using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageToGCode.Engine.GCodeGeneration
{
    class BaseGCode
    {
        public string Comment { get; set; }
        public BaseGCode(string comment)
        {
            Comment = comment;
        }

        public override string ToString()
        {
            return Comment;
        }
    }
}
