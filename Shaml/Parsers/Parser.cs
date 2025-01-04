using System.Collections;
using Shaml.Assigners;
using Shaml.Extension;
using Shaml.Tokens;

namespace Shaml.Parsers;

public class Parser
{
	private const char LF = '\n';
	private const char CR = '\r';
	private const char TAB = '\t';
	
	private const string CompositeScalarFeature = "\"\"\"";

	private const char Slash = '/';
	private const char Backslash = '\\';
	private const char Dollar = '$';
	private const char Asterisk = '*';
	private const char DoubleQuotes = '"';
	private const char OpenBrace = '{';
	private const char CloseBrace = '}';
	private const char Tab = '\t';
	private const char Hyphen = '-';
	private const char Colon = ':';
	private const char Space = ' ';
	private const string StartItem = "- ";
	private const string DelimiterPair = ": ";
	
    private ReadOnlyMemory<char> _m_buffer;
    
    private Parser(ReadOnlyMemory<char> m_buffer)
    {
	    _m_buffer = m_buffer;
    }
    
    public static Node Parse(ReadOnlyMemory<char> m_buffer, Dictionary<string, Cache> globalContext)
    {
	    Parser parser = new(m_buffer);
	    
	    Row[] rows = parser.ToMark(m_buffer);
	    
	    RowParser rowParser = new(rows, m_buffer, globalContext);
	    
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

    		Mark mark = new Mark(startPositionRow, endPositionRow - 1, MarkType.Row);

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
		private Dictionary<string, Cache> _globalContext;
		
		internal RowParser(Row[] rows, ReadOnlyMemory<char> m_buffer, 
			Dictionary<string, Cache> globalContext)
		{
			ArgumentNullException.ThrowIfNull(rows);
			ArgumentNullException.ThrowIfNull(globalContext);
			
			_rows = rows;
			_m_buffer = m_buffer;

			_globalContext = globalContext;
			
			Reset();
		}

		internal Node Parse()
		{
			Node node = new(_m_buffer);

			ParseNode(node, 0);

			Reset();
			
			return node;
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

				int startMark = mark.Start + indentLevel * $"{Tab}".Length;
				int lengthMark = mark.Length - indentLevel * $"{Tab}".Length;

				ReadOnlySpan<char> s_token = s_buffer.Slice(
					start: startMark,
					length: lengthMark);
				
				
				/// Skip empty row.
				if (s_token.IsWhiteSpace() || s_token.IsEmpty)
				{
					continue;
				}
				
				if ((s_token[0], s_token[1]) is (Hyphen, Space))
				{
					IndexReference reference = new(_index, _m_buffer);

					Mark valueMark = new(row.Mark.Start + 3, row.Mark.End);

					Scalar scalar = ParseScalar(_m_buffer, valueMark);

					node.Children.Add(reference, scalar);

					if (!HasNext())
					{
						return node;
					}

					if (row.IndentLevel < indentLevel)
					{
						return node;
					}
					
					continue;
				}

				char lastCharacter = s_token[^1];
				
				switch (lastCharacter)
				{
					case Hyphen:
					{
						IndexReference reference = new(_index, _m_buffer);

						Node innerNode = new(_m_buffer);

						if (!MoveNext())
						{
							throw new Exception($"Item on row [{_index}] is empty.");
						}

						ParseNode(innerNode, indentLevel + 1);

						node.Children.Add(reference, innerNode);
					
						continue;
					}
					case Colon:
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
					
						continue;
					}
				}


				int delimiterPairPosition = s_token.IndexOf(DelimiterPair);
				
				if (delimiterPairPosition != -1)
				{
					int endKey = delimiterPairPosition - 1;

					Mark keyMark = new(startMark, startMark + endKey);
					StaticReference reference = new(keyMark, _m_buffer);

					Mark entireValueMark = new(keyMark.End + 3, row.Mark.End);
					
					Scalar scalar = ParseScalar(_m_buffer, entireValueMark);

					node.Children.Add(reference, scalar);
					
					continue;
				}

				throw new Exception($"Not detected features on row [{_index}][{s_token.ToString()}].");
				
				
			} while (MoveNext());
		    
			return node;
		}

		private Scalar ParseScalar(ReadOnlyMemory<char> buffer, Mark valueMark)
		{
			var span = buffer.Span;
			
			int availableTokenLength = valueMark.End;

			/// Reference token represents 3 symbols: '$', '{' and '}'.
			/// If a mark length less than count this symbols then skip
			/// to detected references.
			if (availableTokenLength < 3)
			{
				return new Scalar(buffer, _globalContext)
				{
					Entire = valueMark,
					Marks = new[] { valueMark }
				};
			}

			List<Mark> marks = new();
			
			int start = valueMark.Start;
			int end = start;
			
			for (; end <= availableTokenLength; end++)
			{
				switch (span[end])
				{
					case Dollar:
					{
						if (span[end + 1] is OpenBrace)
						{
							end += 2;
							
							for (; end <= availableTokenLength; end++)
							{
								if (span[end] is CloseBrace)
								{
									Mark mark = new(start, end, MarkType.Reference);

									marks.Add(mark);

									start = end + 1;

									goto nextMark;
								}
							}
						}
					
						throw new Exception($"Not detected an token end of reference: [{span.Slice(start, valueMark.End).ToString()}].");
					}
					
					default:
					{
						for (; end < availableTokenLength; end++)
						{
							switch (span[end])
							{
								case Dollar:
								{
									if (end + 1 >= availableTokenLength)
									{
										break;
									}
									
									switch (span[end + 1])
									{
										case OpenBrace:
											marks.Add(new Mark(start, end - 1, MarkType.Scalar));

											start = end;
											
											for (end = start; end <= availableTokenLength; end++)
											{
												if (span[end] is CloseBrace)
												{
													Mark mark = new(start, end, MarkType.Reference);

													marks.Add(mark);

													start = end + 1;

													goto nextMark;
												}
											}
											
											throw new Exception($"Not detected an token end of reference: [{span.Slice(start, end - start - 1).ToString()}].");
										
										case Dollar:
											
											if (end + 2 <= availableTokenLength && span[end + 2] is OpenBrace)
											{
												marks.Add(new Mark(start, end - 1, MarkType.Scalar));
												marks.Add(new Mark(end, MarkType.Shield));

												start = end + 1;
												end = end + 2;
											}
											break;
									}

									break;
								}
							}
							
						}
						
						marks.Add(new Mark(start, end));
						
						break;
					}
				}
				
				nextMark: ;
			}
			
			return new Scalar(buffer, _globalContext)
			{
				Marks = marks.ToArray(),
				Entire = valueMark,
			};
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