using System.Collections.Generic;
using Newtonsoft.Json;

namespace IQueryable._01.E3SClient.Entities
{
	[E3SMetaType("meta:people-suite:people-api:com.epam.e3s.app.people.api.data.EmployeeEntity")]
	public class EmployeeEntity : E3SEntity
	{
		[JsonProperty("entityBoost")]
		double EntityBoost { get; set; }

		[JsonProperty("documentBoost")]
		double DocumentBoost { get; set; }

	    [JsonProperty("phone")]
	    List<string> Phone { get; set; }

	    [JsonProperty("skill")]
		Skills Skill { get; set; }

		[JsonProperty("firstname")]
		List<string> Firstname { get; set; }

		[JsonProperty("lastname")]
		List<string> Lastname { get; set; }

		[JsonProperty("fullname")]
		List<string> Fullname { get; set; }

		[JsonProperty("country")]
		List<string> Country { get; set; }

		[JsonProperty("city")]
		List<string> City { get; set; }

		[JsonProperty("email")]
		List<string> Email { get; set; }

		[JsonProperty("skype")]
		List<string> Skype { get; set; }

		[JsonProperty("social")]
		List<string> Social { get; set; }

		[JsonProperty("attachment")]
		List<string> Attachment { get; set; }

		[JsonProperty("manager")]
		public string Manager { get; set; }

		[JsonProperty("superior")]
		public string Superior { get; set; }

		[JsonProperty("startworkdate")]
		public string StartWorkDate { get; set; }

		[JsonProperty("project")]
		public string Project { get; set; }

		[JsonProperty("projectall")]
		public string Projectall { get; set; }

		[JsonProperty("trainer")]
		List<string> Trainer { get; set; }

		[JsonProperty("kb")]
		List<string> Kb { get; set; }

		[JsonProperty("certificate")]
		public string Certificate { get; set; }

		[JsonProperty("unit")]
		public string Unit { get; set; }

		[JsonProperty("office")]
		public string Office { get; set; }

		[JsonProperty("room")]
		public string Room { get; set; }

		[JsonProperty("status")]
		public string Status { get; set; }

		[JsonProperty("car")]
		public string Car { get; set; }

		[JsonProperty("birthday")]
		public string Birthday { get; set; }

		[JsonProperty("workhistory")]
		public List<WorkHistory> Workhistory { get; set; }

		[JsonProperty("jobfunction")]
		List<string> JobFunction { get; set; }

		[JsonProperty("recognition")]
		List<Recognition> Recognition { get; set; }

		[JsonProperty("badge")]
		List<string> Badge { get; set; }

		[JsonProperty("dismissal")]
		public string Dismissal { get; set; }

		[JsonProperty("endProbationDate")]
		public string EndProbationDate { get; set; }

		[JsonProperty("endworkdate")]
		public string EndWorkDate { get; set; }

		[JsonProperty("errupdatedate")]
		public string ErrUpdateDate { get; set; }

		[JsonProperty("edulevel")]
		public string EduLevel { get; set; }

		[JsonProperty("eduschool")]
		public string EduSchool { get; set; }

		[JsonProperty("edufield")]
		public string EduField { get; set; }

		[JsonProperty("eduendyear")]
		public string EduEndYear { get; set; }

		[JsonProperty("workstation")]
		public string Workstation { get; set; }

		[JsonProperty("nativename")]
		public string NativeName { get; set; }

		[JsonProperty("governmentalid")]
		public string GovernmentalId { get; set; }

		[JsonProperty("billable")]
		public double Billable { get; set; }

		[JsonProperty("nonbillable")]
		public double Nonbillable { get; set; }
	}
}
