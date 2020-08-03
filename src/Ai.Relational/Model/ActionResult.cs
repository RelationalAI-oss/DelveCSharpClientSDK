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
using JsonSubTypes;
using System.ComponentModel.DataAnnotations;
using OpenAPIDateConverter = Ai.Relational.Client.OpenAPIDateConverter;

namespace Ai.Relational.Model
{
    /// <summary>
    /// ActionResult
    /// </summary>
    [DataContract]
    [JsonConverter(typeof(JsonSubtypes), "objtp")]
    [JsonSubtypes.KnownSubType(typeof(CardinalityActionResult), "CardinalityActionResult")]
    [JsonSubtypes.KnownSubType(typeof(ModifyWorkspaceActionResult), "ModifyWorkspaceActionResult")]
    [JsonSubtypes.KnownSubType(typeof(CollectProblemsActionResult), "CollectProblemsActionResult")]
    [JsonSubtypes.KnownSubType(typeof(ParseActionResult), "ParseActionResult")]
    [JsonSubtypes.KnownSubType(typeof(SetOptionsActionResult), "SetOptionsActionResult")]
    [JsonSubtypes.KnownSubType(typeof(InstallActionResult), "InstallActionResult")]
    [JsonSubtypes.KnownSubType(typeof(ListSourceActionResult), "ListSourceActionResult")]
    [JsonSubtypes.KnownSubType(typeof(UpdateActionResult), "UpdateActionResult")]
    [JsonSubtypes.KnownSubType(typeof(LoadDataActionResult), "LoadDataActionResult")]
    [JsonSubtypes.KnownSubType(typeof(QueryActionResult), "QueryActionResult")]
    [JsonSubtypes.KnownSubType(typeof(ListEdbActionResult), "ListEdbActionResult")]
    [JsonSubtypes.KnownSubType(typeof(ImportActionResult), "ImportActionResult")]
    public partial class ActionResult :  IEquatable<ActionResult>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionResult" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected ActionResult() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionResult" /> class.
        /// </summary>
        /// <param name="objtp">objtp (required) (default to &quot;&quot;).</param>
        public ActionResult(string objtp = "")
        {
            // to ensure "objtp" is required (not null)
            if (objtp == null)
            {
                throw new InvalidDataException("objtp is a required property for ActionResult and cannot be null");
            }
            else
            {
                this.Objtp = objtp;
            }
            
        }
        
        /// <summary>
        /// Gets or Sets Objtp
        /// </summary>
        [DataMember(Name="objtp", EmitDefaultValue=true)]
        public string Objtp { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ActionResult {\n");
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
            return this.Equals(input as ActionResult);
        }

        /// <summary>
        /// Returns true if ActionResult instances are equal
        /// </summary>
        /// <param name="input">Instance of ActionResult to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ActionResult input)
        {
            if (input == null)
                return false;

            return 
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
            return this.BaseValidate(validationContext);
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        protected IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> BaseValidate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
