using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using Raven.Abstractions.Indexing;

namespace Raven.Database.Indexing.IndexMerging
{
    internal class IndexData
    {
        public override string ToString()
        {
            return string.Format("IndexName: {0}", IndexName);
        }

        private readonly IndexDefinition index;
        public Dictionary<string, Expression> SelectExpressions = new Dictionary<string, Expression>();

        public IndexData(IndexDefinition index)
        {
            this.index = index;
        }

        public Expression FromExpression { get; set; }
        public string FromIdentifier { get; set; }
        public int NumberOfFromClauses { get; set; }
        public int NumberOfSelectClauses { get; set; }

        public string OriginalMap { get; set; }
        public bool HasWhere { get; set; }
        public bool HasLet { get; set; }
        public bool HasGroup { get; set; }
        public bool HasOrder { get; set; }

        public int IndexId { get; set; }
        public string IndexName { get; set; }
        public bool IsAlreadyMerged { get; set; }
        public bool IsSuitedForMerge { get; set; }
        public string Comment { get; set; }

        public string Collection { get; set; }

        public IDictionary<string, FieldStorage> Stores
        {
            get { return index.Stores; }
        }
        public IDictionary<string, FieldIndexing> Indexes
        {
            get { return index.Indexes; }
        }
        public IDictionary<string, SortOptions> SortOptions
        {
            get { return index.SortOptions; }
        }
        public IDictionary<string, string> Analyzers
        {
            get { return index.Analyzers; }
        }
        public ISet<string> SuggestionsOptions
        {
            get { return index.SuggestionsOptions; }
        }
        public IDictionary<string, FieldTermVector> TermVectors
        {
            get { return index.TermVectors; }
        }
        public IDictionary<string, SpatialOptions> SpatialIndexes
        {
            get { return index.SpatialIndexes; }
        }

        public IndexDefinition Index
        {
            get { return index; }
        }
        public bool IsMapReduceOrMultiMap { get; set; }

        public string BuildExpression(Dictionary<string, Expression> selectExpressions)
        {
            var anonymousTypeCreateExpression = new AnonymousTypeCreateExpression();
            var crrv = new ChangeRootReferenceVisitor(FromIdentifier);
            foreach (var curExpr in selectExpressions.OrderBy(x => x.Key))
            {
                curExpr.Value.AcceptVisitor(crrv);
                anonymousTypeCreateExpression.Initializers.Add(
                    new AssignmentExpression(new IdentifierExpression(curExpr.Key), curExpr.Value.Clone()));
            }
            if (FromExpression == null)
                FromExpression = new IdentifierExpression();

            var queryExpr = new QueryExpression
            {
                Clauses =
                {
                    new QueryFromClause
                    {
                        Identifier = "doc",
                        Expression = FromExpression.Clone()
                    },
                    new QuerySelectClause
                    {
                        Expression = anonymousTypeCreateExpression.Clone()
                    }
                }
            };
            FromIdentifier = "doc";

            var printer = new StringWriter();
            var printerVisitor = new CSharpOutputVisitor(printer, FormattingOptionsFactory.CreateSharpDevelop());
            queryExpr.AcceptVisitor(printerVisitor);

            var format = printer.GetStringBuilder().ToString();
            if (format.Substring(0, 3) == "\r\n\t")
            {
                format = format.Remove(0, 3);
            }
            format = format.Replace("\r\n\t", "\n");
            return format;
        }
    }
}
