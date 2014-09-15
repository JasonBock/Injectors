using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Visitors;
using Injectors.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
using NR = ICSharpCode.NRefactory.Ast;

namespace Injectors.Core.Attributes
{
	internal sealed class NotNullAttributeDebugger
	{
		internal NotNullAttributeDebugger(MethodDefinition method, ParameterDefinition target)
		{
			this.SetPoint(method, target);
		}

		private void SetPoint(MethodDefinition method, ParameterDefinition target)
		{
			var point = method.FindSequencePoint();

			if(point != null)
			{
				using(var parser = ParserFactory.CreateParser(
					point.Document.Url))
				{
					parser.Parse();

					if(parser.Errors.Count <= 0)
					{
						var visitor = new NotNullAttributeVisitor(
							point.Document, method, target);
						parser.CompilationUnit.AcceptVisitor(visitor, null);
						this.SequencePoint = visitor.SequencePoint;
					}
				}
			}
		}

		internal SequencePoint SequencePoint { get; private set; }

		private sealed class NotNullAttributeVisitor : AbstractAstVisitor
		{
			internal NotNullAttributeVisitor(Document document, MethodDefinition method, ParameterDefinition target)
			{
				this.Document = document;
				this.Method = method;
				this.Parameter = target;
			}

			public override object VisitConstructorDeclaration(NR.ConstructorDeclaration constructorDeclaration, object data)
			{
				this.VisitParametrizedNode(constructorDeclaration, true);
				return base.VisitConstructorDeclaration(constructorDeclaration, data);
			}

			public override object VisitMethodDeclaration(NR.MethodDeclaration methodDeclaration, object data)
			{
				this.VisitParametrizedNode(methodDeclaration, false);
				return base.VisitMethodDeclaration(methodDeclaration, data);
			}

			private void VisitParametrizedNode(NR.ParametrizedNode node, bool isConstructor)
			{
				if(((isConstructor && this.Method.IsConstructor) || (node.Name == this.Method.Name)) &&
					node.Parameters.Count == this.Method.Parameters.Count)
				{
					var doParametersMatch = true;
					NR.ParameterDeclarationExpression matchingParameter = null;

					for(var i = 0; i < node.Parameters.Count; i++)
					{
						var parsedParameter = node.Parameters[i];

						if(parsedParameter.ParameterName != this.Method.Parameters[i].Name)
						{
							doParametersMatch = false;
							break;
						}
						else if(parsedParameter.ParameterName == this.Parameter.Name)
						{
							matchingParameter = parsedParameter;
						}
					}

					if(doParametersMatch && matchingParameter != null)
					{
						this.SequencePoint = (from attributeSection in matchingParameter.Attributes
													 from attribute in attributeSection.Attributes
													 where (attribute.Name == "NotNullAttribute" || attribute.Name == "NotNull")
													 select new SequencePoint(this.Document)
													 {
														 EndColumn = attribute.EndLocation.Column,
														 EndLine = attribute.EndLocation.Line,
														 StartColumn = attribute.StartLocation.Column,
														 StartLine = attribute.StartLocation.Line
													 }).Single();
					}
				}
			}

			private Document Document { get; set; }
			private MethodDefinition Method { get; set; }
			private ParameterDefinition Parameter { get; set; }
			internal SequencePoint SequencePoint { get; private set; }
		}
	}
}
