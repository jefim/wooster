using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wooster.Utils;

namespace Wooster.Utils
{
    public abstract class ViewModelBase : Observable
    {
        public virtual string DisplayName { get { return "ViewModelBase"; } }
    }
}
