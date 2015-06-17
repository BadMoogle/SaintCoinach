﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaintCoinach.Text.Nodes {
    public class GenericElement : INode, INodeWithChildren, INodeWithArguments, IExpressionNode {
        private TagType _Tag;
        private ArgumentCollection _Arguments;
        private INode _Content;

        public TagType Tag { get { return _Tag; } }
        public IEnumerable<INode> Arguments { get { return _Arguments; } }
        public INode Content { get { return _Content; } }
        NodeFlags INode.Flags {
            get {
                var f = NodeFlags.IsExpression;
                if (_Arguments.HasItems)
                    f |= NodeFlags.HasArguments;
                if (Content != null)
                    f |= NodeFlags.HasChildren;
                return f;
            }
        }

        public GenericElement(TagType tag, INode content, params INode[] arguments) : this(tag, content, (IEnumerable<INode>)arguments) { }
        public GenericElement(TagType tag, INode content, IEnumerable<INode> arguments) {
            _Tag = tag;
            _Arguments = new ArgumentCollection(arguments);
            _Content = content;
        }

        public bool Equals(INode other) {
            var n = other as GenericElement;
            if (n == null)
                return false;
            if (_Tag != n._Tag)
                return false;
            if (!_Arguments.Equals(n._Arguments))
                return false;

            var lNull = object.ReferenceEquals(_Content, null);
            var rNull = object.ReferenceEquals(n._Content, null);
            if (lNull != rNull)
                return false;
            if (lNull)
                return true;
            return _Content.Equals(n._Content);
        }
        public int CompareTo(INode other) {
            var n = other as GenericElement;
            if (n == null)
                return 1;
            if (_Tag != n._Tag)
                return ((byte)_Tag).CompareTo((byte)n._Tag);

            var argsCmp = _Arguments.CompareTo(n._Arguments);
            if (argsCmp != 0)
                return argsCmp;

            {
                var lNull = object.ReferenceEquals(_Content, null);
                var rNull = object.ReferenceEquals(n._Content, null);
                if (lNull != rNull)
                    return lNull.CompareTo(rNull);
                if (lNull)
                    return 0;

                var cntCmp = _Content.CompareTo(n._Content);
                if (cntCmp != 0)
                    return cntCmp;
            }

            return 0;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            ToString(sb);
            return sb.ToString();
        }
        public void ToString(StringBuilder builder) {
            builder.Append(StringTokens.TagOpen);
            builder.Append(Tag);

            _Arguments.ToString(builder);

            if (Content == null) {
                builder.Append(StringTokens.ElementClose);
                builder.Append(StringTokens.TagClose);
            } else {
                builder.Append(StringTokens.TagClose);

                Content.ToString(builder);

                builder.Append(StringTokens.TagOpen);
                builder.Append(StringTokens.ElementClose);
                builder.Append(Tag);
                builder.Append(StringTokens.TagClose);
            }
        }

        #region INodeWithChildren Members

        IEnumerable<INode> INodeWithChildren.Children {
            get { yield return Content; }
        }

        #endregion

        #region IExpressionNode Members

        public IExpression Evaluate(IEvaluationFunctionProvider provider, EvaluationParameters parameters) {
            return provider.EvaluateGenericElement(parameters, this);
        }

        #endregion
    }
}
