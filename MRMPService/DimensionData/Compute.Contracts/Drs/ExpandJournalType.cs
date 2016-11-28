using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD.CBU.Compute.Api.Contracts.Drs
{
	/// 
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34283")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "urn:didata.com:api:cloud:types")]
	[System.Xml.Serialization.XmlRootAttribute("expandJournal", Namespace = "urn:didata.com:api:cloud:types", IsNullable = true)]
	public partial class ExpandJournalType
	{
		private int sizeGbField;
		private string idField;
		/// 
		public int sizeGb
		{
			get
			{
				return this.sizeGbField;
			}
			set
			{
				this.sizeGbField = value;
			}
		}
		/// 
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string id
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
			}
		}
	}
}
