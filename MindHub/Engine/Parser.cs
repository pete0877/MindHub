using System;
using System.Web.UI;
using System.Collections;
using System.Collections.Specialized;

namespace Engine
{
	/// <summary>
	/// Base parser class used to parse templates.
	/// </summary>
	public class Parser : IParser
	{
		/// <summary>
		/// Variable used to represent base path to UI objects (value of this variable contains language and style codes).
		/// </summary>
		static public string VAR_UIPATHWEB			= "cuipathweb";
		
		/// <summary>
		/// Template being parsed.
		/// </summary>
		protected ITemplate m_template;

		/// <summary>
		/// Transaction used for parsing.
		/// </summary>
		protected Transaction m_transaction;

		/// <summary>
		/// List of constants used on every parsing (E.g. VAR_UIPATHWEB)
		/// </summary>
		protected NameValueCollection m_constants;

		/// <summary>
		/// List of name-value collections used for variable-drawing.
		/// </summary>
		protected ArrayList m_inputNameValueCollections;

		/// <summary>
		/// List of hashtables used for variable-drawing.
		/// </summary>
		protected ArrayList m_inputHashtables;

		/// <summary>
		/// List of data objects used for variable-drawing.
		/// </summary>
		protected ArrayList m_inputDataObjects;

		/// <summary>
		/// List of data readers used for variable-drawing.
		/// </summary>
		protected ArrayList m_inputDataReaders;

		/// <summary>
		/// Session used for variable-drawing.
		/// </summary>
		protected Engine.Session m_inputSession;

		/// <summary>
		/// Language code for which this parsing is taking place.
		/// </summary>
		protected string m_languageCd;

		/// <summary>
		/// Style code for which this parsing is taking place.
		/// </summary>
		protected string m_styleCd;

		/// <summary>
		/// Web path to the UI folder (includes language and style codes - e.g. '/fake/eng/orange').
		/// </summary>
		protected string m_uiPathWeb;

		/// <summary>
		/// List of tags found within the template (comes from TemplateMetaData).
		/// </summary>
		protected ArrayList m_tags;

		/// <summary>
		/// List of expressions found within the template (comes from TemplateMetaData).
		/// </summary>
		protected ArrayList m_expressions;

		/// <summary>
		/// List of plain HTML buffers found within the template (comes from TemplateMetaData).
		/// </summary>
		protected ArrayList m_htmlBuffers;

		/// <summary>
		/// Index into the m_tags hash that caused the parser to stop.
		/// </summary>
		protected int		m_stopPosition;

		/// <summary>
		/// List of output buffers (contains fully parsed HTML code for HTML sections and parsed tags).
		/// </summary>
		protected ArrayList m_outputBuffers;

		/// <summary>
		/// Flag for if entire template has been parsed already.
		/// </summary>
		protected bool m_parsingCompleted;

		/// <summary>
		/// Cached template metadata.
		/// </summary>
		protected TemplateMetaData m_templateMetaData;

		/// <summary>
		/// Flag for if the template has been initialized (metadata + expression trees)
		/// </summary>
		protected bool m_initialized;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="template">Template being parsed</param>
		/// <param name="transaction">Transaction to be used for parsing</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public Parser(ITemplate template, Transaction transaction)
		{
			// Validate
			if (template == null)
				throw new EInvalidParameter(this, "template");

			// Create and initialize member variables:
			m_template = template;
			m_transaction = transaction;
			m_languageCd = template.GetLanguageCd();
			m_styleCd = template.GetStyleCd();
			m_uiPathWeb = template.GetUIPathWeb();
			m_constants = new NameValueCollection();
			m_inputNameValueCollections = new ArrayList();
			m_inputHashtables = new ArrayList();
			m_inputDataObjects = new ArrayList();
			m_inputDataReaders = new ArrayList();
			m_templateMetaData = null;
			m_tags = null;
			m_expressions = null;
			m_htmlBuffers = null;
			m_inputSession = null;
			m_stopPosition = 0;
			m_parsingCompleted = false;			

			m_initialized = false;

			// Establish values that will always be available within each template parsed:
			SetContsants();
		}

		/// <summary>
		/// Initializes template meta-data and creates expression trees.
		/// </summary>
		void Initialize()
		{
			if (m_initialized)
				return;

			m_templateMetaData = m_template.GetMetaData();
			m_tags = m_templateMetaData.Tags;
			m_htmlBuffers = m_templateMetaData.HTMLBuffers;
			m_expressions = m_templateMetaData.Expressions;
			if (m_expressions == null)
			{
				m_expressions = new ArrayList();
				IEnumerator itr = m_tags.GetEnumerator();
				while (itr.MoveNext())
				{
					string tag = (string) itr.Current;
					Expression expression = BuildExpression(tag);	
					m_expressions.Add(expression);
				}			
				m_templateMetaData.Expressions = m_expressions;
			}

			m_initialized = true;
		}


		/// <summary>
		/// Returns name of the template being parsed.
		/// Performs get by reference (unsafe).
		/// </summary>
		/// <returns>Template name (e.g. filename of a file template)</returns>
		public string GetTemplateName()
		{
			return m_template.GetName();
		}


		/// <summary>
		/// Returns the language within which this parser is performing all operations.
		/// </summary>
		/// <returns>Language code</returns>
		public string GetLanguageCd()
		{
			return m_languageCd;
		}		


		/// <summary>
		/// Returns the transaction object used to do parsing.
		/// </summary>
		/// <returns>Transaction object</returns>
		public Transaction GetTransaction()
		{
			return m_transaction;
		}	


		/// <summary>
		/// Populates m_constants with constants (e.g. VAR_UIPATHWEB).
		/// </summary>
		virtual protected void SetContsants()
		{
			m_constants[VAR_UIPATHWEB] = m_uiPathWeb;
		}


		/// <summary>
		/// Adds input for variable-drawing.
		/// </summary>
		/// <param name="input">Input to be added to variable-drawing</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public void AddInput(Engine.Session input)
		{
			if (input == null)
				throw new EInvalidParameter(this, "input");

			m_inputSession = input;
		}


		/// <summary>
		/// Adds input for variable-drawing.
		/// </summary>
		/// <param name="input">Input to be added to variable-drawing</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public void AddInput(Hashtable input)
		{
			if (input == null)
				throw new EInvalidParameter(this, "input");

			m_inputHashtables.Add(input);
		}


		/// <summary>
		/// Adds input for variable-drawing.
		/// </summary>
		/// <param name="input">Input to be added to variable-drawing</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public void AddInput(NameValueCollection input)
		{
			if (input == null)
				throw new EInvalidParameter(this, "input");

			m_inputNameValueCollections.Add(input);
		}


		/// <summary>
		/// Adds input for variable-drawing.
		/// </summary>
		/// <param name="input">Input to be added to variable-drawing</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public void AddInput(IDataObject input)
		{
			if (input == null)
				throw new EInvalidParameter(this, "input");

			m_inputDataObjects.Add(input);
		}


		/// <summary>
		/// Adds input for variable-drawing.
		/// </summary>
		/// <param name="input">Input to be added to variable-drawing</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public void AddInput(DataReader input)
		{
			if (input == null)
				throw new EInvalidParameter(this, "input");
		

			m_inputDataReaders.Add(input);
		}


		/// <summary>
		/// Output entire template into given output wihtout parsing.
		/// </summary>
		/// <param name="output">Output</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public void ReadTemplate(HtmlTextWriter output)
		{
			output.Write(m_template.GetContent());
		}


		/// <summary>
		/// Parses entire template into given output.
		/// </summary>
		/// <param name="output">Output</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public void ReadTemplate(string output)
		{
			output += m_template.GetContent();
		}


		/// <summary>
		/// Parses entire template into given output.
		/// </summary>
		/// <param name="output">Output</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public void Parse(HtmlTextWriter output)
		{
			Parse(output, null);
		}


		/// <summary>
		/// Parses entire template into given output.
		/// </summary>
		/// <param name="output">Output</param>
		/// <exception cref="EInvalidParameter">If any parameters are null</exception>
		public void Parse(out string output)
		{			
			Parse(out output, null);
		}

		
		/// <summary>
		/// Parses entire template into given output. Stops if encounters stopTag. 
		/// </summary>
		/// <param name="output">Output</param>
		/// <param name="stopTag">If stopTag is null or empty, template will be ignored and whole (or rest) of the template will be parsed.</param>
		/// <exception cref="EInvalidParameter">If [output] parameter is null</exception>
		public void Parse(HtmlTextWriter output, string stopTag)
		{
			Initialize();

			if (output == null)
				throw new EInvalidParameter(this, "output");

			if (m_parsingCompleted)
				return;

			// Algoritm is this:
			// * Output first HTML buffer 
			// Check if first tag is same as top tag, if yes, stop.
			// (else) parse the expression at the first position and output put it 
			// Move on to next position. Go back to *:

			while (!m_parsingCompleted)
			{
				if (m_stopPosition < m_htmlBuffers.Count)
					output.Write(m_htmlBuffers[m_stopPosition]);

				if (
						stopTag != null && 
						stopTag.Length > 0 && 
						m_stopPosition < m_tags.Count &&
						(string) m_tags[m_stopPosition] == stopTag
				)
				{
					m_stopPosition++;
					break;
				}

				// Else, we have not encountered stop-tag so we evaluate the tag expression:
				if (m_stopPosition < m_tags.Count)
				{
					DataValue evaluationResult = EvaluateExpression((Expression) m_expressions[m_stopPosition]);
					output.Write(evaluationResult.GetStringByReference());
				}

				m_stopPosition++;

				// Check if we finished parsing the tempalte:
				if (m_stopPosition >= m_tags.Count && m_stopPosition >= m_htmlBuffers.Count)
					m_parsingCompleted = true;
			}
		}


		/// <summary>
		/// Parses entire template into given output. Stops if encounters stopTag. 
		/// </summary>
		/// <param name="output">Output</param>
		/// <param name="stopTag">If stopTag is null or empty, template will be ignored and whole (or rest) of the template will be parsed.</param>
		/// <exception cref="EInvalidParameter">If [output] parameter is null</exception>
		public void Parse(out string output, string stopTag)
		{			
			Initialize();

			output = "";

			if (m_parsingCompleted)
				return;

			// Algoritm is this:
			// * Output first HTML buffer 
			// Check if first tag is same as top tag, if yes, stop.
			// (else) parse the expression at the first position and output put it 
			// Move on to next position. Go back to *:

			while (!m_parsingCompleted)
			{
				if (m_stopPosition < m_htmlBuffers.Count)
					output += m_htmlBuffers[m_stopPosition];

				if (
					stopTag != null && 
					stopTag.Length > 0 && 
					m_stopPosition < m_tags.Count &&
					(string) m_tags[m_stopPosition] == stopTag
				)
				{
					m_stopPosition++;
					break;
				}

				// Else, we have not encountered stop-tag so we evaluate the tag expression:
				if (m_stopPosition < m_tags.Count)
				{
					DataValue evaluationResult = EvaluateExpression((Expression) m_expressions[m_stopPosition]);
					output += evaluationResult.GetStringByReference();
				}

				m_stopPosition++;

				// Check if we finished parsing the tempalte:
				if (m_stopPosition >= m_tags.Count && m_stopPosition >= m_htmlBuffers.Count)
					m_parsingCompleted = true;
			}
		}


		/// <summary>
		/// Evaluates expression into DataValue using added inputs and registered function objects.
		/// </summary>
		/// <param name="expression">Expression object</param>
		/// <returns>Value of the evaluated expression.
		public DataValue EvaluateExpression(Expression expression)
		{
			if (expression.Kind == Expression.ExpressionKind.DataValue)
				return expression.Value;

			if (expression.Kind == Expression.ExpressionKind.Variable)
				return EvaluateVariable(expression.Name);

			// Else we deal with function:
			string functionName = expression.Name;
			ITemplateFunction functionObject = (ITemplateFunction) TemplateFunctionFactory.GetFactory().Produce(functionName);

			// Evaluate all children:
            IEnumerator itr = expression.Children.GetEnumerator();
			ArrayList arguments = new ArrayList();
			while (itr.MoveNext())
			{
				Expression argumentExpression = (Expression) itr.Current;
				DataValue argumentDataValue = EvaluateExpression(argumentExpression);
				arguments.Add(argumentDataValue);
			}
			DataValue evaluatedResult = functionObject.Evaluate(arguments, m_transaction, this, m_inputSession);

			return evaluatedResult;
		}


		/// <summary>
		/// Evaluates variable into DataValue using added inputs.
		/// </summary>
		/// <param name="varibleName">Variable name</param>
		/// <returns>DataValue found in one of the inputs or new DataValue("") if none of the inputs contain the variable.</returns>
		public DataValue EvaluateVariable(string varibleName)
		{
			// Presedance of value collections (lower number = higher)
			// 0. constants (hard-coded by this class)
			// 1. table hashes
			// 2. data objects
			// 3. data readers
			// 4. value collections
			// 5. session
			
			// Check constants:
			string variableValue = m_constants[varibleName];
			if (variableValue != null)
				return variableValue;

			// Check all table hashes:
			IEnumerator itrTableHashes = m_inputHashtables.GetEnumerator();
			while (itrTableHashes.MoveNext())
			{
				Hashtable hashTable = (Hashtable) itrTableHashes.Current;
				if (hashTable.ContainsKey(varibleName))
					return (string) hashTable[varibleName];
			}

			// Check all data objects:
			IEnumerator itrDataObjects = m_inputDataObjects.GetEnumerator();
			while (itrDataObjects.MoveNext())
			{
				IDataObject dataObject = (IDataObject) itrDataObjects.Current;
				Hashtable dataObjectElements = dataObject.GetElements();
				if (dataObjectElements.ContainsKey(varibleName))
				{
					DataElement element = (DataElement) dataObjectElements[varibleName];
					return element.GetValue();
				}
			}

			// Check all data readers:
			IEnumerator itrDataReaders = m_inputDataReaders.GetEnumerator();
			while (itrDataReaders.MoveNext())
			{
				DataReader dataReader = (DataReader) itrDataReaders.Current;
				if (dataReader.IsOnValidPosition() && dataReader.Contains(varibleName))
					return dataReader[varibleName];
			}

			// Check name-value collections:
			IEnumerator itrNameValueCollections = m_inputNameValueCollections.GetEnumerator();
			while (itrNameValueCollections.MoveNext())
			{
				NameValueCollection collection = (NameValueCollection) itrNameValueCollections.Current;
				variableValue = collection[varibleName];
				if (variableValue != null)
					return variableValue;
			}

			// Check session:			
			if (m_inputSession != null)
			{
				object sessionObject = m_inputSession[varibleName];
				if (sessionObject == null)
					return "";

				return sessionObject.ToString();
			}

			return "";
		}


		/// <summary>
		/// Set variable in the current parsing context.
		/// </summary>
		/// <param name="variableName">Name of variable</param>
		/// <param name="variableValue">Value to be used</param>
		public void SetVariable(string variableName, DataValue variableValue)
		{
			if (variableName == null)
				throw new EInvalidParameter(this, "variableName");

			if (variableValue == null)
				throw new EInvalidParameter(this, "variableValue");

 			m_constants[variableName] = variableValue;
		}


		/// <summary>
		/// Builds expression structure by parsing (creating expression / parse tree) the expression string.
		/// </summary>
		/// <param name="expressionString">String containing expression</param>
		/// <returns>Built expression</returns>
		/// <exception cref="ESyntaxError">If expression string cannot be evaluated.</exception>
		protected Expression BuildExpression(string expressionString)
		{
			// case: empty string
			if (expressionString.Length == 0)
				return new Expression(Expression.ExpressionKind.DataValue, null, "", null);

			// case: expression begins with a string so the whole expression must be quoted string:
			if (expressionString[0] == '\"')
			{
				if (expressionString.Length < 2)
					throw new ESyntaxError(this, m_template.GetName(), expressionString, "Missing closing double-quote sign to end string");

				return new Expression(Expression.ExpressionKind.DataValue, null, expressionString.Substring(1, expressionString.Length - 2), null);
			}

			// case: expression begins with a digit so the whole expression must be a number:
			if (expressionString[0] >= '0' && expressionString[0] <= '9')
			{
				Expression result = new Expression();
				result.Kind = Expression.ExpressionKind.DataValue;
				
				// Check if we are daling with with double or integer
				int firstDotPosition = expressionString.IndexOf('.');
				if (firstDotPosition < 0) // no dot means whole number
					result.Value = new DataValue(expressionString).GetLong();
				else
					result.Value = new DataValue(expressionString).GetDouble();

				return result;
			}

			// Case: expression contains left pren (means whole expression is a function)
			// Case: expression does not contain left parent (means whole expression is a variable)
			int firstLeftParenPos = expressionString.IndexOf('(');
			if (firstLeftParenPos < 0) // no left-paren in the expression .. must be variable
				return new Expression(Expression.ExpressionKind.Variable, expressionString, null, null);

			// We must be evaluating a function. Look for the right parenthesis:
			Expression expression = new Expression();
			expression.Kind = Expression.ExpressionKind.Function;
			expression.Name = expressionString.Substring(0, firstLeftParenPos).ToLower();
			int lastRightParenPos = expressionString.LastIndexOf(')');
			if (lastRightParenPos < 0)
				throw new ESyntaxError(this, m_template.GetName(), expressionString, "Missing right paren.");

			string expressionListString = expressionString.Substring(firstLeftParenPos + 1, lastRightParenPos - firstLeftParenPos - 1);
			expression.Children = BuildExpressionList(expressionListString);

			return expression;
		}


		/// <summary>
		/// Builds list of expressions from comma-delimited expressions string list. 
		/// For exemple:  3, b(a()), "\"" has 3 expressions.
		/// </summary>
		/// <param name="expressionListString">String list containing expressions.</param>
		/// <returns>List of Expression objects.</returns>
		/// <exception cref="ESyntaxError">If expression string cannot be evaluated.</exception>
		protected ArrayList BuildExpressionList(string expressionListString)
		{
			ArrayList result = new ArrayList();

			// All this function needs to do is to find true commas separating expressions and call BuildExpression
			// on each splitted off expression. For this we simply have to watch out for strings (enclosed within double 
			// quote signs) and possibly nested functions.
			// E.g. " some, text\" ", A(",", B(0 , 6)) has only two expressions that need to be returned.
			// Methodology here is to cruise through the expressionListString and recognize true commas.

			string list = expressionListString.ToLower();

			// Case: no arguments
			if (list.Length == 0)
				return result;

			// By adding comma to the list we ensure that the last argument in the list gets treated
			// without extra handling outside of the main loop:
			list += ",";

			int position = 0;
			int lastPostDelimiterPosition = 0;
			bool inQuote = false;
			int functionDepth = 0;
			while(position < list.Length)
			{
				char current = list[position];
				if (current == '\"')
				{
					if (position == 0) // List begins with quote sign
						inQuote = true;
					else
					{
						// If we are already inside quoted string, we are either getting out of it or have one
						// that has been escaped:
						if (inQuote)
						{
							if (list[position - 1] != '\\') // Previous character is not escape so we are closing quoted string:
								inQuote = false;
						}
						else // We are entering quoted mode:
							inQuote = true;

					}
				}
				else if (current == '(')
				{
					// If we are not within quoted string, then this means going down into sub-function:
					if (!inQuote)
						functionDepth++;
				}
				else if (current == ')')
				{
					// If we are not within quoted string, then this means going out from sub-function:
					if (!inQuote)
						functionDepth--;

					if (functionDepth < 0) // This technically should never happen.
						functionDepth = 0;
				}
				else if (current == ',')
				{
					// Commas only count as expression delimiters if they are not within quotes 
					// and we are not currently at a level of a sub-funciton:
					if (!inQuote && functionDepth == 0)
					{
						// We found expresison delimiter. 
						string expressionString = expressionListString.Substring(lastPostDelimiterPosition, position - lastPostDelimiterPosition);
						expressionString = expressionString.Trim();
						// If we dealt with a quoted string, we shall convert escaped characters:
						if (expressionString.Length > 0 && expressionString[0] == '\"')
							expressionString = expressionString.Replace("\\\"", "\"");

						lastPostDelimiterPosition = position + 1;

						Expression expression = BuildExpression(expressionString);
						result.Add(expression);
					}
				}
				position++;
			}

			return result;
		}
	}
}

