using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
    internal interface IClearable
    {
        /// <summary>
        /// Cleares the resource.
        /// </summary>
        void Clear();

        /// <summary>
        /// Check if the resource is empty.
        /// </summary>
        bool IsFull();

        /// <summary>
        /// Check if the resource is empty.
        /// </summary>
        bool IsEmpty();
    }
}
