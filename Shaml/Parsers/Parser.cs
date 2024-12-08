using System.Collections;
using Shaml.Extension;
using Shaml.Tokens;

namespace Shaml.Parsers;

public class Parser
{
	private const char LF = '\n';
	private const char CR = '\r';
	private const char TAB = '\t';

	private const string CompositeScalarFeature = "\"\"\"";
	
    private ReadOnlyMemory<char> _m_buffer;
    
    private Parser(ReadOnlyMemory<char> m_buffer)
    {
	    _m_buffer = m_buffer;
    }
    
    public static Node Parse(ReadOnlyMemory<char> m_buffer)
    {
	    Parser parser = new(m_buffer);
	    
	    Row[] rows = parser.ToMark(m_buffer);
	    
	    
	    RowParser rowParser = new(rows, m_buffer);
	    
	    Node node = rowParser.Parse();
	    
	    return node;
    }
    
    private Row[] ToMark(ReadOnlyMemory<char> m_buffer)
    {
    	List<Row> rows = new();
	    
	    ReadOnlySpan<char> s_buffer = m_buffer.Span;
    	
    	int startPositionRow = 0;

    	for (int endPositionRow = 0; endPositionRow < s_buffer.Length; endPositionRow++)
    	{
    		int indentLevel = 0;
    		
    		for (; endPositionRow < s_buffer.Length; endPositionRow++)
    		{
    			if (s_buffer[endPositionRow] is TAB)
    			{
    				indentLevel++;
    				continue;
    			}
    			
    			break;
    		}

    		Eol eol = Eol.END;
    		
    		for (; endPositionRow < s_buffer.Length; endPositionRow++)
    		{
    			switch (s_buffer[endPositionRow])
    			{
    				case CR:
    				{
    					eol = Eol.CR;

    					if (endPositionRow + 1 < s_buffer.Length &&
					        s_buffer[endPositionRow + 1] is LF)
    					{
    						eol = Eol.CRLF;
    						endPositionRow++;
    					}

    					goto nextRow;
    				}

    				case LF:
    				{
    					eol = Eol.LF;
    					goto nextRow;
    				}

    				default:
    					continue;
    			}
    		}

    		nextRow:

    		Mark mark = new Mark(startPositionRow, endPositionRow - 1);

    		Row row = new(m_buffer, mark, indentLevel, eol);

    		rows.Add(row);

    		startPositionRow = endPositionRow + 1;
    	}
    	
    	return rows.ToArray();
    }
    
    private class RowParser
	{
		private int _index = 0;
		
		private readonly Row[] _rows;
		private readonly ReadOnlyMemory<char> _m_buffer;
	
		internal RowParser(Row[] rows, ReadOnlyMemory<char> m_buffer)
		{
			ArgumentNullException.ThrowIfNull(rows);

			_rows = rows;
			_m_buffer = m_buffer;

			Reset();
		}

		internal Node Parse()
		{
			Node node = new(_m_buffer);

			ParseNode(node, 0);

			Reset();
			
			return node;
		}

		private Node ParseCollection(Node node, int indentLevel)
		{
			return node;
		}

		private Node ParseCompositeScalar()
		{
			return null;
		}
		private Node ParseNode(Node node, int indentLevel)
		{
			ReadOnlySpan<char> s_buffer = _m_buffer.Span;

			do
			{
				Row row = Current;

				if (row.IndentLevel < indentLevel)
				{
					_index--;
					
					return node;
				}

				Mark mark = row.Mark;

				if (row.Mark.Length < 2)
				{
					continue;
				}

				int startMark = mark.Start + indentLevel;
				int lengthMark = mark.Length - indentLevel;
				
				ReadOnlySpan<char> s_token = s_buffer.Slice(
					start: startMark,
					length: lengthMark);

				if (s_token.IsWhiteSpace() || s_token.IsEmpty)
				{
					continue;
				}

				
				if (s_token.Slice(0, 2) is "- ")
				{
					IndexReference reference = new(_index, _m_buffer);

					Mark valueMark = new(row.Mark.Start + 2, row.Mark.End);

					Scalar scalar = new(_m_buffer) { Value = valueMark };

					node.Children.Add(reference, scalar);

					if (!MoveNext())
					{
						return node;
					}

					if (row.IndentLevel < indentLevel)
					{
						return node;
					}
				}
				else if (s_token[^1] is '-')
				{
					IndexReference reference = new(_index, _m_buffer);

					Node innerNode = new(_m_buffer);

					if (!MoveNext())
					{
						throw new Exception($"Item on row [{_index}] is empty.");
					}

					ParseNode(innerNode, indentLevel + 1);

					node.Children.Add(reference, innerNode);
				}
				else if (s_token[^1] is ':')
				{
					Mark keyMark = new(row.Mark.Start + indentLevel, row.Mark.End - 1);

					StaticReference reference = new(keyMark, _m_buffer);
					
					if (!HasNext())
					{
						node.Children.Add(reference, null);
						
						return node;
					}

					MoveNext();
					

					Row nextRow = Current;

					int compositeLevel = indentLevel + 1;
					
					ReadOnlySpan<char> next_s_token = s_buffer.Slice(nextRow.Mark.Start + compositeLevel, nextRow.Mark.Length - compositeLevel);

					if (next_s_token is CompositeScalarFeature)
					{
						int startCompositeScalar = _index + 1;
						int endCompositeScalar = -1;

						while (MoveNext())
						{
							nextRow = Current;

							ReadOnlySpan<char> s_token_end = s_buffer.Slice(nextRow.Mark.Start + compositeLevel,
								nextRow.Mark.Length - compositeLevel);

							/// if (endCompositeScalar_Row.IndentLevel >= indentLevel) ok
						
							if (s_token_end is CompositeScalarFeature)
							{
								endCompositeScalar = _index - 1;
								break;
							}
						
							if (!HasNext())
							{
								throw new Exception(
									$"Parser detected START composite scalar feature on row [{_index}]. But not detected END.");
							}
						}
						
						Mark[] marks = new Mark[endCompositeScalar - startCompositeScalar + 1];
						
						for (int i = startCompositeScalar, j = 0; j < marks.Length; i++, j++)
						{
							Row itemRow = _rows[i];

							/// Deleting indent (tabs)
							marks[j] = new(itemRow.Mark.Start + compositeLevel, itemRow.Mark.End);
						}

						node.Children.Add(reference, new CompositeScalar(_m_buffer, marks));
						
						continue;
					}
					
					
					Node innerNode = new(_m_buffer);
					
					ParseNode(innerNode, indentLevel + 1);
					
					node.Children.Add(reference, innerNode);
				}
				else if (s_token.IndexOf(": ") != -1)
				{
					int endKey = startMark + s_token.IndexOf(": ") - 1;

					Mark keyMark = new(startMark, endKey);
					StaticReference reference = new(keyMark, _m_buffer);

					Mark valueMark = new(endKey + 3, row.Mark.End);
					Scalar scalar = new(_m_buffer) { Value = valueMark };
					
					node.Children.Add(reference, scalar);
				}
				else
				{
					throw new Exception($"Not detected features on row [{_index}][{s_token.ToString()}].");
				}


				;
			} while (MoveNext());
		    
			return node;
		}

		private bool HasNext() => _index + 1 < _rows.Length;

		private bool MoveNext()
		{
			if (HasNext())
			{
				_index++;
				return true;
			}

			return false;
		}

		private void Reset() => _index = 0;
		private Row Current => _rows[_index];
	} 
}