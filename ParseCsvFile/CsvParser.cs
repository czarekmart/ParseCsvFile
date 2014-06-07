using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseCsvFile
{
	public class CsvParser
	{
		private List<CsvSection>			_sections;

		public CsvParser()
		{
			_sections = new List<CsvSection>();
		}

		public CsvSection LookupSection ( int sectionCode )
		{
			return _sections.FirstOrDefault(s=>s.SectionCode == sectionCode);
		}

		public CsvSection ParseSection ( string headerLine, Func<string> lineProvider )
		{
			var headerTags = HeaderLineParser(headerLine);

			return SectionParserStarter ( lineProvider(), headerTags, lineProvider );
		}

		private CsvSection SectionParserStarter ( string dataLine, List<string> lastHeaderTags, Func<string> lineProvider )
		{
			//-- determine number of expeced tags
			List<string> dataTags = CsvSection.ParseDataLine ( dataLine );
			int equipmentCode = Convert.ToInt32 ( dataTags[1] );
			var section = LookupSection ( equipmentCode );
 			if ( section == null	)
			{
				_sections.Add ( section = new CsvSection(equipmentCode, lastHeaderTags) );
				section.Parse ( dataLine, lineProvider );
			}
			else
			{
				section.Parse ( dataLine, lineProvider );
			}
			return section;
		}

		public List<string> HeaderLineParser ( string headerLine )
		{
			//var result = new List<string>();

			throw new NotImplementedException("HeaderLineParser not implemented.");

			//return result;
		}
	}

	public class CsvSection
	{
		public int						SectionCode			{ get; private set; }
		public List<string>				Header				{ get; private set; }
		public List<List<string>>		Data				{ get; private set; }
		public bool						Eof					{ get; private set; }
		public string					SectionEndLine		{ get; private set; }

		public int						TagCount
		{
			get { return Header.Count; }
		}

		public CsvSection ( int sectionCode, List<string> header )
		{
			SectionCode = sectionCode;
			Header = header;
			Data = new List<List<string>>();
			Eof = false;
			SectionEndLine = null;
		}
		public static List<string> ParseDataLine ( string dataLine )
		{
			throw new NotImplementedException("ParseDataLine not implemented.");

			// Remember not to include open quotes in the result list.

			//List<string> dataTags = new List<string>();

			//

			//return dataTags;
		}

		private List<string> ParseDataLine ( string dataLine, Func<string> lineProvider )
		{
			if ( dataLine == null )
			{
				Eof = true;
				return null;
			}
			else if ( !IsDataLine(dataLine) )
			{
				return null;
			}
			else
			{
				var dataTags = ParseDataLine(dataLine);
				if ( dataTags.Count >= TagCount )
				{
					return dataTags;
				}
				else
				{
					//-- Not enough tags. Borrow from next line.
					string nextLine = lineProvider();
					if ( nextLine == null )
					{
						Eof = true;
						return dataTags;
					}
					else
					{
						return ParseDataLine(dataLine+nextLine, lineProvider);
					}
				}
			}
		}

		private bool IsDataLine ( string line )
		{
			return !(string.IsNullOrEmpty(line) ||  line.StartsWith("//") || line.StartsWith("\"//") || line.StartsWith(","));
		}

		public CsvSection Parse ( string dataLine, Func<string> lineProvider )
		{
			for ( var dataTags = ParseDataLine(dataLine, lineProvider); !Eof; dataTags = ParseDataLine(lineProvider(), lineProvider) )
			{
				int eqpCode = -1;
				if ( dataTags == null || dataTags.Count < 2 || !int.TryParse(dataTags[1], out eqpCode) || eqpCode != SectionCode )
				{
					//-- end of section
					SectionEndLine = dataLine;
					break;
				}
				else
				{
					Data.Add ( dataTags );
				}
			}
			return this;
		}
	}
}
