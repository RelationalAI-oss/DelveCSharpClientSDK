/* 
 * Delve Client SDK
 *
 * This is a Client SDK for Delve API
 *
 * The version of the OpenAPI document: 1.0.0
 * Contact: support@relational.ai
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using OpenAPIDateConverter = Ai.Relational.Client.OpenAPIDateConverter;

namespace Ai.Relational.Model
{
    /// <summary>
    /// CSVFileSyntax
    /// </summary>
    [DataContract]
    public partial class CSVFileSyntax : FileSyntax,  IEquatable<CSVFileSyntax>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSVFileSyntax" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected CSVFileSyntax() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="CSVFileSyntax" /> class.
        /// </summary>
        /// <param name="header">header.</param>
        /// <param name="headerRow">headerRow.</param>
        /// <param name="normalizenames">normalizenames (default to false).</param>
        /// <param name="datarow">datarow.</param>
        /// <param name="missingstrings">missingstrings.</param>
        /// <param name="delim">delim (default to &quot;&quot;).</param>
        /// <param name="ignorerepeated">ignorerepeated (default to false).</param>
        /// <param name="quotechar">quotechar (default to &quot;&quot;).</param>
        /// <param name="escapechar">escapechar (default to &quot;&quot;).</param>
        public CSVFileSyntax(List<string> header = default(List<string>), ModelInt headerRow = default(ModelInt), bool normalizenames = false, ModelInt datarow = default(ModelInt), List<string> missingstrings = default(List<string>), string delim = "", bool ignorerepeated = false, string quotechar = "", string escapechar = "", string objtp = "") : base(objtp)
        {
            this.Header = header;
            this.HeaderRow = headerRow;
            // use default value if no "normalizenames" provided
            if (normalizenames == null)
            {
                this.Normalizenames = false;
            }
            else
            {
                this.Normalizenames = normalizenames;
            }
            this.Datarow = datarow;
            this.Missingstrings = missingstrings;
            // use default value if no "delim" provided
            if (delim == null)
            {
                this.Delim = "";
            }
            else
            {
                this.Delim = delim;
            }
            // use default value if no "ignorerepeated" provided
            if (ignorerepeated == null)
            {
                this.Ignorerepeated = false;
            }
            else
            {
                this.Ignorerepeated = ignorerepeated;
            }
            // use default value if no "quotechar" provided
            if (quotechar == null)
            {
                this.Quotechar = "";
            }
            else
            {
                this.Quotechar = quotechar;
            }
            // use default value if no "escapechar" provided
            if (escapechar == null)
            {
                this.Escapechar = "";
            }
            else
            {
                this.Escapechar = escapechar;
            }
        }
        
        /// <summary>
        /// Gets or Sets Header
        /// </summary>
        [DataMember(Name="header", EmitDefaultValue=false)]
        public List<string> Header { get; set; }

        /// <summary>
        /// Gets or Sets HeaderRow
        /// </summary>
        [DataMember(Name="header_row", EmitDefaultValue=false)]
        public ModelInt HeaderRow { get; set; }

        /// <summary>
        /// Gets or Sets Normalizenames
        /// </summary>
        [DataMember(Name="normalizenames", EmitDefaultValue=false)]
        public bool Normalizenames { get; set; }

        /// <summary>
        /// Gets or Sets Datarow
        /// </summary>
        [DataMember(Name="datarow", EmitDefaultValue=false)]
        public ModelInt Datarow { get; set; }

        /// <summary>
        /// Gets or Sets Missingstrings
        /// </summary>
        [DataMember(Name="missingstrings", EmitDefaultValue=false)]
        public List<string> Missingstrings { get; set; }

        /// <summary>
        /// Gets or Sets Delim
        /// </summary>
        [DataMember(Name="delim", EmitDefaultValue=false)]
        public string Delim { get; set; }

        /// <summary>
        /// Gets or Sets Ignorerepeated
        /// </summary>
        [DataMember(Name="ignorerepeated", EmitDefaultValue=false)]
        public bool Ignorerepeated { get; set; }

        /// <summary>
        /// Gets or Sets Quotechar
        /// </summary>
        [DataMember(Name="quotechar", EmitDefaultValue=false)]
        public string Quotechar { get; set; }

        /// <summary>
        /// Gets or Sets Escapechar
        /// </summary>
        [DataMember(Name="escapechar", EmitDefaultValue=false)]
        public string Escapechar { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CSVFileSyntax {\n");
            sb.Append("  ").Append(base.ToString().Replace("\n", "\n  ")).Append("\n");
            sb.Append("  Header: ").Append(Header).Append("\n");
            sb.Append("  HeaderRow: ").Append(HeaderRow).Append("\n");
            sb.Append("  Normalizenames: ").Append(Normalizenames).Append("\n");
            sb.Append("  Datarow: ").Append(Datarow).Append("\n");
            sb.Append("  Missingstrings: ").Append(Missingstrings).Append("\n");
            sb.Append("  Delim: ").Append(Delim).Append("\n");
            sb.Append("  Ignorerepeated: ").Append(Ignorerepeated).Append("\n");
            sb.Append("  Quotechar: ").Append(Quotechar).Append("\n");
            sb.Append("  Escapechar: ").Append(Escapechar).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public override string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as CSVFileSyntax);
        }

        /// <summary>
        /// Returns true if CSVFileSyntax instances are equal
        /// </summary>
        /// <param name="input">Instance of CSVFileSyntax to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(CSVFileSyntax input)
        {
            if (input == null)
                return false;

            return base.Equals(input) && 
                (
                    this.Header == input.Header ||
                    this.Header != null &&
                    input.Header != null &&
                    this.Header.SequenceEqual(input.Header)
                ) && base.Equals(input) && 
                (
                    this.HeaderRow == input.HeaderRow ||
                    (this.HeaderRow != null &&
                    this.HeaderRow.Equals(input.HeaderRow))
                ) && base.Equals(input) && 
                (
                    this.Normalizenames == input.Normalizenames ||
                    (this.Normalizenames != null &&
                    this.Normalizenames.Equals(input.Normalizenames))
                ) && base.Equals(input) && 
                (
                    this.Datarow == input.Datarow ||
                    (this.Datarow != null &&
                    this.Datarow.Equals(input.Datarow))
                ) && base.Equals(input) && 
                (
                    this.Missingstrings == input.Missingstrings ||
                    this.Missingstrings != null &&
                    input.Missingstrings != null &&
                    this.Missingstrings.SequenceEqual(input.Missingstrings)
                ) && base.Equals(input) && 
                (
                    this.Delim == input.Delim ||
                    (this.Delim != null &&
                    this.Delim.Equals(input.Delim))
                ) && base.Equals(input) && 
                (
                    this.Ignorerepeated == input.Ignorerepeated ||
                    (this.Ignorerepeated != null &&
                    this.Ignorerepeated.Equals(input.Ignorerepeated))
                ) && base.Equals(input) && 
                (
                    this.Quotechar == input.Quotechar ||
                    (this.Quotechar != null &&
                    this.Quotechar.Equals(input.Quotechar))
                ) && base.Equals(input) && 
                (
                    this.Escapechar == input.Escapechar ||
                    (this.Escapechar != null &&
                    this.Escapechar.Equals(input.Escapechar))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = base.GetHashCode();
                if (this.Header != null)
                    hashCode = hashCode * 59 + this.Header.GetHashCode();
                if (this.HeaderRow != null)
                    hashCode = hashCode * 59 + this.HeaderRow.GetHashCode();
                if (this.Normalizenames != null)
                    hashCode = hashCode * 59 + this.Normalizenames.GetHashCode();
                if (this.Datarow != null)
                    hashCode = hashCode * 59 + this.Datarow.GetHashCode();
                if (this.Missingstrings != null)
                    hashCode = hashCode * 59 + this.Missingstrings.GetHashCode();
                if (this.Delim != null)
                    hashCode = hashCode * 59 + this.Delim.GetHashCode();
                if (this.Ignorerepeated != null)
                    hashCode = hashCode * 59 + this.Ignorerepeated.GetHashCode();
                if (this.Quotechar != null)
                    hashCode = hashCode * 59 + this.Quotechar.GetHashCode();
                if (this.Escapechar != null)
                    hashCode = hashCode * 59 + this.Escapechar.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            foreach(var x in base.BaseValidate(validationContext)) yield return x;
            yield break;
        }
    }

}
