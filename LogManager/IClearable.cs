using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
    /// <summary>
    /// Defines a way to clear the resource and to test if it is full or empty.
    /// </summary>
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