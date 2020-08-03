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
    /// ListSourceActionResult
    /// </summary>
    [DataContract]
    public partial class ListSourceActionResult : ActionResult,  IEquatable<ListSourceActionResult>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListSourceActionResult" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected ListSourceActionResult() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ListSourceActionResult" /> class.
        /// </summary>
        /// <param name="sources">sources.</param>
        public ListSourceActionResult(List<Source> sources = default(List<Source>), string objtp = "") : base(objtp)
        {
            this.Sources = sources;
        }
        
        /// <summary>
        /// Gets or Sets Sources
        /// </summary>
        [DataMember(Name="sources", EmitDefaultValue=false)]
        public List<Source> Sources { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ListSourceActionResult {\n");
            sb.Append("  ").Append(base.ToString().Replace("\n", "\n  ")).Append("\n");
            sb.Append("  Sources: ").Append(Sources).Append("\n");
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
            return this.Equals(input as ListSourceActionResult);
        }

        /// <summary>
        /// Returns true if ListSourceActionResult instances are equal
        /// </summary>
        /// <param name="input">Instance of ListSourceActionResult to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ListSourceActionResult input)
        {
            if (input == null)
                return false;

            return base.Equals(input) && 
                (
                    this.Sources == input.Sources ||
                    this.Sources != null &&
                    input.Sources != null &&
                    this.Sources.SequenceEqual(input.Sources)
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
                if (this.Sources != null)
                    hashCode = hashCode * 59 + this.Sources.GetHashCode();
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
