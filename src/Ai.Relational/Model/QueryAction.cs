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
    /// QueryAction
    /// </summary>
    [DataContract]
    public partial class QueryAction : Action,  IEquatable<QueryAction>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryAction" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected QueryAction() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryAction" /> class.
        /// </summary>
        /// <param name="source">source (required).</param>
        /// <param name="inputs">inputs.</param>
        /// <param name="persist">persist.</param>
        /// <param name="outputs">outputs.</param>
        public QueryAction(Source source = default(Source), RelDict inputs = default(RelDict), List<string> persist = default(List<string>), List<string> outputs = default(List<string>), string objtp = "") : base(objtp)
        {
            // to ensure "source" is required (not null)
            if (source == null)
            {
                throw new InvalidDataException("source is a required property for QueryAction and cannot be null");
            }
            else
            {
                this.Source = source;
            }
            
            this.Inputs = inputs;
            this.Persist = persist;
            this.Outputs = outputs;
        }
        
        /// <summary>
        /// Gets or Sets Source
        /// </summary>
        [DataMember(Name="source", EmitDefaultValue=true)]
        public Source Source { get; set; }

        /// <summary>
        /// Gets or Sets Inputs
        /// </summary>
        [DataMember(Name="inputs", EmitDefaultValue=false)]
        public RelDict Inputs { get; set; }

        /// <summary>
        /// Gets or Sets Persist
        /// </summary>
        [DataMember(Name="persist", EmitDefaultValue=false)]
        public List<string> Persist { get; set; }

        /// <summary>
        /// Gets or Sets Outputs
        /// </summary>
        [DataMember(Name="outputs", EmitDefaultValue=false)]
        public List<string> Outputs { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class QueryAction {\n");
            sb.Append("  ").Append(base.ToString().Replace("\n", "\n  ")).Append("\n");
            sb.Append("  Source: ").Append(Source).Append("\n");
            sb.Append("  Inputs: ").Append(Inputs).Append("\n");
            sb.Append("  Persist: ").Append(Persist).Append("\n");
            sb.Append("  Outputs: ").Append(Outputs).Append("\n");
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
            return this.Equals(input as QueryAction);
        }

        /// <summary>
        /// Returns true if QueryAction instances are equal
        /// </summary>
        /// <param name="input">Instance of QueryAction to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(QueryAction input)
        {
            if (input == null)
                return false;

            return base.Equals(input) && 
                (
                    this.Source == input.Source ||
                    (this.Source != null &&
                    this.Source.Equals(input.Source))
                ) && base.Equals(input) && 
                (
                    this.Inputs == input.Inputs ||
                    (this.Inputs != null &&
                    this.Inputs.Equals(input.Inputs))
                ) && base.Equals(input) && 
                (
                    this.Persist == input.Persist ||
                    this.Persist != null &&
                    input.Persist != null &&
                    this.Persist.SequenceEqual(input.Persist)
                ) && base.Equals(input) && 
                (
                    this.Outputs == input.Outputs ||
                    this.Outputs != null &&
                    input.Outputs != null &&
                    this.Outputs.SequenceEqual(input.Outputs)
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
                if (this.Source != null)
                    hashCode = hashCode * 59 + this.Source.GetHashCode();
                if (this.Inputs != null)
                    hashCode = hashCode * 59 + this.Inputs.GetHashCode();
                if (this.Persist != null)
                    hashCode = hashCode * 59 + this.Persist.GetHashCode();
                if (this.Outputs != null)
                    hashCode = hashCode * 59 + this.Outputs.GetHashCode();
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
