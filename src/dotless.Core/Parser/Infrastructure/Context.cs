namespace dotless.Core.Parser.Infrastructure
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Tree;
    using Utils;

    public class Context : IEnumerable<IEnumerable<Selector>>
    {
        private List<List<Selector>> Paths { get; set; }

        public Context()
        {
            Paths = new List<List<Selector>>();
        }

        public void AppendSelectors(Context context, IList<Selector> selectors)
        {
            if (context == null || context.Paths.Count == 0)
            {
                Paths.AddRange(selectors.Select(s => new List<Selector> { s }));
                return;
            }

            for (var i = 0; i < selectors.Count; i++)
            {
                var selector = selectors[i];

                var subPaths = context.Paths.SelectList(path => { var ret = new List<Selector>(path); ret.Add(selector); return ret; });
                Paths.AddRange(subPaths);
            }
        }

        public string ToCSS(Env env)
        {
            return Paths
                .Select(p => p.Select(s => s.ToCSS(env)).JoinStrings("").Trim())
                .JoinStrings(env.Compress ? "," : (Paths.Count > 3 ? ",\n" : ", "));
        }

        public int Count
        {
            get { return Paths.Count; }
        }

        public IEnumerator<IEnumerable<Selector>> GetEnumerator()
        {
            return Paths.Cast<IEnumerable<Selector>>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}