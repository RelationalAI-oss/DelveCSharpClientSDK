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
    /// Point
    /// </summary>
    [DataContract]
    public partial class Point :  IEquatable<Point>, IValidatableObject
    {
        /// <summary>
        /// Defines Objtp
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ObjtpEnum
        {
            /// <summary>
            /// Enum Point for value: Point
            /// </summary>
            [EnumMember(Value = "Point")]
            Point = 1

        }

        /// <summary>
        /// Gets or Sets Objtp
        /// </summary>
        [DataMember(Name="objtp", EmitDefaultValue=true)]
        public ObjtpEnum Objtp { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Point" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected Point() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Point" /> class.
        /// </summary>
        /// <param name="row">row.</param>
        /// <param name="column">column.</param>
        /// <param name="objtp">objtp (required) (default to ObjtpEnum.Point).</param>
        public Point(ModelInt row = default(ModelInt), ModelInt column = default(ModelInt), ObjtpEnum objtp = ObjtpEnum.Point)
        {
            // to ensure "objtp" is required (not null)
            if (objtp == null)
            {
                throw new InvalidDataException("objtp is a required property for Point and cannot be null");
            }
            else
            {
                this.Objtp = objtp;
            }
            
            this.Row = row;
            this.Column = column;
        }
        
        /// <summary>
        /// Gets or Sets Row
        /// </summary>
        [DataMember(Name="row", EmitDefaultValue=false)]
        public ModelInt Row { get; set; }

        /// <summary>
        /// Gets or Sets Column
        /// </summary>
        [DataMember(Name="column", EmitDefaultValue=false)]
        public ModelInt Column { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Point {\n");
            sb.Append("  Row: ").Append(Row).Append("\n");
            sb.Append("  Column: ").Append(Column).Append("\n");
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
            return this.Equals(input as Point);
        }

        /// <summary>
        /// Returns true if Point instances are equal
        /// </summary>
        /// <param name="input">Instance of Point to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Point input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Row == input.Row ||
                    (this.Row != null &&
                    this.Row.Equals(input.Row))
                ) && 
                (
                    this.Column == input.Column ||
                    (this.Column != null &&
                    this.Column.Equals(input.Column))
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
                if (this.Row != null)
                    hashCode = hashCode * 59 + this.Row.GetHashCode();
                if (this.Column != null)
                    hashCode = hashCode * 59 + this.Column.GetHashCode();
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
