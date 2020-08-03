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
    /// LabeledActionResult
    /// </summary>
    [DataContract]
    public partial class LabeledActionResult :  IEquatable<LabeledActionResult>, IValidatableObject
    {
        /// <summary>
        /// Defines Objtp
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ObjtpEnum
        {
            /// <summary>
            /// Enum LabeledActionResult for value: LabeledActionResult
            /// </summary>
            [EnumMember(Value = "LabeledActionResult")]
            LabeledActionResult = 1

        }

        /// <summary>
        /// Gets or Sets Objtp
        /// </summary>
        [DataMember(Name="objtp", EmitDefaultValue=true)]
        public ObjtpEnum Objtp { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="LabeledActionResult" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected LabeledActionResult() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="LabeledActionResult" /> class.
        /// </summary>
        /// <param name="name">name (default to &quot;&quot;).</param>
        /// <param name="result">result (required).</param>
        /// <param name="objtp">objtp (required) (default to ObjtpEnum.LabeledActionResult).</param>
        public LabeledActionResult(string name = "", ActionResult result = default(ActionResult), ObjtpEnum objtp = ObjtpEnum.LabeledActionResult)
        {
            // to ensure "result" is required (not null)
            if (result == null)
            {
                throw new InvalidDataException("result is a required property for LabeledActionResult and cannot be null");
            }
            else
            {
                this.Result = result;
            }
            
            // to ensure "objtp" is required (not null)
            if (objtp == null)
            {
                throw new InvalidDataException("objtp is a required property for LabeledActionResult and cannot be null");
            }
            else
            {
                this.Objtp = objtp;
            }
            
            // use default value if no "name" provided
            if (name == null)
            {
                this.Name = "";
            }
            else
            {
                this.Name = name;
            }
        }
        
        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name="name", EmitDefaultValue=false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets Result
        /// </summary>
        [DataMember(Name="result", EmitDefaultValue=true)]
        public ActionResult Result { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class LabeledActionResult {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Result: ").Append(Result).Append("\n");
            sb.Append("  Objtp: ").Append(Objtp).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
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
            return this.Equals(input as LabeledActionResult);
        }

        /// <summary>
        /// Returns true if LabeledActionResult instances are equal
        /// </summary>
        /// <param name="input">Instance of LabeledActionResult to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(LabeledActionResult input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Name == input.Name ||
                    (this.Name != null &&
                    this.Name.Equals(input.Name))
                ) && 
                (
                    this.Result == input.Result ||
                    (this.Result != null &&
                    this.Result.Equals(input.Result))
                ) && 
                (
                    this.Objtp == input.Objtp ||
                    (this.Objtp != null &&
                    this.Objtp.Equals(input.Objtp))
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
                int hashCode = 41;
                if (this.Name != null)
                    hashCode = hashCode * 59 + this.Name.GetHashCode();
                if (this.Result != null)
                    hashCode = hashCode * 59 + this.Result.GetHashCode();
                if (this.Objtp != null)
                    hashCode = hashCode * 59 + this.Objtp.GetHashCode();
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
            yield break;
        }
    }

}
