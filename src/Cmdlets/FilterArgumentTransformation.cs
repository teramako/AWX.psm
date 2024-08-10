using System.Collections;
using System.Collections.Specialized;
using System.Management.Automation;
using System.Web;

namespace AWX.Cmdlets
{
    /// <summary>
    /// Transform <c>-Filter</c> parameter values to <see cref="NameValueCollection"/>
    /// </summary>
    public class FilterArgumentTransformationAttribute : ArgumentTransformationAttribute
    {
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            var queries = HttpUtility.ParseQueryString("");

            foreach (var item in ToItems(inputData))
            {
                switch (item)
                {
                    case Filter filter:
                        queries.Add(filter.GetKey(), filter.Value);
                        continue;
                    case IDictionary dict:
                        {
                            var f = Filter.Parse(dict);
                            queries.Add(f.GetKey(), f.Value);
                        }
                        continue;
                    case NameValueCollection nvc:
                        foreach (var f in GetQueries(nvc))
                        {
                            queries.Add(f.GetKey(), f.Value);
                        }
                        continue;
                    case string str:
                        foreach (var f in GetQueries(HttpUtility.ParseQueryString(str)))
                        {
                            queries.Add(f.GetKey(), f.Value);
                        }
                        continue;
                    default:
                        continue;
                }
            }
            return queries;
        }
        private static IEnumerable ToItems(object inputData)
        {
            if (inputData is IList list)
            {
                foreach (var item in list)
                {
                    yield return item;
                }
            }
            else
            {
                yield return inputData;
            }
        }
        private static IEnumerable<Filter> GetQueries(NameValueCollection collection)
        {
            foreach (string key in collection.Keys)
            {
                var values = collection.GetValues(key);
                if (values == null) continue;
                foreach (var val in values)
                {
                    yield return new Filter(key, val);
                }
            }
        }
    }
}
