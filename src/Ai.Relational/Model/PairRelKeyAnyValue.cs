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
    /// PairRelKeyAnyValue
    /// </summary>
    [DataContract]
    public partial class PairRelKeyAnyValue :  IEquatable<PairRelKeyAnyValue>, IValidatableObject
    {
        /// <summary>
        /// Defines Objtp
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ObjtpEnum
        {
            /// <summary>
            /// Enum PairRelKeyAnyValue for value: Pair_RelKey_AnyValue
            /// </summary>
            [EnumMember(Value = "Pair_RelKey_AnyValue")]
            PairRelKeyAnyValue = 1

        }

        /// <summary>
        /// Gets or Sets Objtp
        /// </summary>
        [DataMember(Name="objtp", EmitDefaultValue=true)]
        public ObjtpEnum Objtp { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="PairRelKeyAnyValue" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected PairRelKeyAnyValue() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="PairRelKeyAnyValue" /> class.
        /// </summary>
        /// <param name="first">first (required).</param>
        /// <param name="second">second.</param>
        /// <param name="objtp">objtp (required) (default to ObjtpEnum.PairRelKeyAnyValue).</param>
        public PairRelKeyAnyValue(RelKey first = default(RelKey), AnyValue second = default(AnyValue), ObjtpEnum objtp = ObjtpEnum.PairRelKeyAnyValue)
        {
            // to ensure "first" is required (not null)
            if (first == null)
            {
                throw new InvalidDataException("first is a required property for PairRelKeyAnyValue and cannot be null");
            }
            else
            {
                this.First = first;
            }
            
            // to ensure "objtp" is required (not null)
            if (objtp == null)
            {
                throw new InvalidDataException("objtp is a required property for PairRelKeyAnyValue and cannot be null");
            }
            else
            {
                this.Objtp = objtp;
            }
            
            this.Second = second;
        }
        
        /// <summary>
        /// Gets or Sets First
        /// </summary>
        [DataMember(Name="first", EmitDefaultValue=true)]
        public RelKey First { get; set; }

        /// <summary>
        /// Gets or Sets Second
        /// </summary>
        [DataMember(Name="second", EmitDefaultValue=false)]
        public AnyValue Second { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class PairRelKeyAnyValue {\n");
            sb.Append("  First: ").Append(First).Append("\n");
            sb.Append("  Second: ").Append(Second).Append("\n");
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
            return this.Equals(input as PairRelKeyAnyValue);
        }

        /// <summary>
        /// Returns true if PairRelKeyAnyValue instances are equal
        /// </summary>
        /// <param name="input">Instance of PairRelKeyAnyValue to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(PairRelKeyAnyValue input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.First == input.First ||
                    (this.First != null &&
                    this.First.Equals(input.First))
                ) && 
                (
                    this.Second == input.Second ||
                    (this.Second != null &&
                    this.Second.Equals(input.Second))
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
                if (this.First != null)
                    hashCode = hashCode * 59 + this.First.GetHashCode();
                if (this.Second != null)
                    hashCode = hashCode * 59 + this.Second.GetHashCode();
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
