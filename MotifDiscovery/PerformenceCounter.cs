using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotifDiscovery
{
    /// <summary>
    /// This class is used for preprocessing scv data
    /// </summary>
    class PerformenceCounter
    {
        /// <summary>
        /// Used for storing the meta data
        /// </summary>
        private List<String> PerformenceCounterAttributes = new List<string>();

        /// <summary>
        /// Construction Method
        /// </summary>
        /// <param name="request">The imput resource as a string</param>
        public PerformenceCounter(String request)
        {
            this.InitialRequestAttributes(request);
        }

        /// <summary>
        /// Get the i-th value of the request list
        /// </summary>
        /// <param name="index"> THe index of the data</param>
        /// <returns></returns>
        public String getAttributeValue(int index)
        {
            return this.PerformenceCounterAttributes[index];
        }

        /// <summary>
        /// Return the counter size
        /// </summary>
        /// <returns></returns>
        public int CounterSize()
        {
            return this.PerformenceCounterAttributes.Count;
        }

        /// <summary>
        /// initial the request Attribute list
        /// </summary>
        /// <param name="request"></param>
        private void InitialRequestAttributes(String request)
        {
            String attribute = "";
            for (int i = 0; i < request.Length; i++)
            {

                if (request[i].Equals(','))
                {
                    PerformenceCounterAttributes.Add(attribute);
                    attribute = "";
                }
                else
                {
                    attribute += request[i];
                }
            }
        }
    }
}
