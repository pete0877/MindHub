using System;
using System.Collections;

namespace Engine
{
	/// <summary>
	/// Class responsible for analyzing template content and generating meta-data for it (e.g. list of tags, their posistions, lenghts, etc).
	/// </summary>
	public class TemplateMetaData
	{
		/// <summary>
		/// Flag for if this instance has been initialize (template analyzed).
		/// </summary>
		protected bool m_initialized;

		/// <summary>
		/// Array of pointers (off-sets within the template content) to tags (e.g. m_tagPointers[2] stores starting position of the third tag).
		/// </summary>
		public ArrayList TagPointers;		

		/// <summary>
		/// Array of tags (e.g. m_tags[2] stores third tag - e.g. "SomeFunction('', 'blah')"). Values are trimmed.
		/// </summary>
		public ArrayList Tags;

		/// <summary>
		/// Array of expression trees (populated by the parser)
		/// </summary>
		public ArrayList Expressions;

		/// <summary>
		/// Array of plain HTML content
		/// </summary>
		public ArrayList HTMLBuffers;

		/// <summary>
		/// Constructor.
		/// </summary>
		public TemplateMetaData()
		{
			TagPointers = new ArrayList();
			Tags = new ArrayList();
			HTMLBuffers = new ArrayList();
			Expressions = null;
			m_initialized = false;
		}


		/// <summary>
		/// Initializes this instance of the meta-data. 
		/// </summary>
		/// <param name="template"></param>
		public void Initialize(ITemplate template)
		{
			// Plow through entire stream of characters to fish out <% %> tags.
			int htmlBufferStartPosition = 0;
			char[] content = template.GetContent();
			for (int startPosition = 0; startPosition < content.Length; startPosition++)
			{
				// Find start tag <%
				if (content[startPosition] == '<' && (startPosition + 1 < content.Length) && content[startPosition + 1] == '%')
				{
					// Since we encountered a tag, we need to take whatever is before it and 
					// cache it as HTML buffer:
					string htmlBuffer = "";
					if (startPosition > 0)
						htmlBuffer = new string(content, htmlBufferStartPosition, startPosition - htmlBufferStartPosition);

					HTMLBuffers.Add(htmlBuffer);

					// Look for end-tag ( %> )
					bool foundEnd = false;
					int endPosition = startPosition + 3; // We not point at character after <%  ... (e.g. '!' in <%%!)
					while (endPosition < content.Length)
					{
						if (content[endPosition] == '>' && content[endPosition - 1] == '%')
						{
							foundEnd = true;
							break;
						}

						endPosition++;
					}

					// If we found both start and end-tags, we can record position and length of the tag:
					if (foundEnd)
					{
						// Mark new starting position of HTML buffer:
						htmlBufferStartPosition = endPosition + 1;

						// At this point, if we take the start, and add the lenght we can easily skip over the whole tag
						int fullLenght = endPosition + 1 - startPosition; // Includes <% and %>
						// Remeber position where <% starts:
						TagPointers.Add(startPosition);

						// When adding to list of tags, we want to skip the <% and %> so that parser doesn't have to worry about it:
						string tagCode = new string(content, startPosition + 2, fullLenght - 4);
						tagCode = tagCode.Trim();
						Tags.Add(tagCode);
						startPosition = endPosition; // Now we start-point at character > (of %>) 
					}
					else 
						break;
				}
			}

			// Put the final buffer in place:
			string htmlBufferLast = new string(content, htmlBufferStartPosition, content.Length - htmlBufferStartPosition);
			HTMLBuffers.Add(htmlBufferLast);

			m_initialized = true;
		}
	}
}
