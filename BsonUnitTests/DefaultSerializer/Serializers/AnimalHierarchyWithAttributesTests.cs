﻿/* Copyright 2010 10gen Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

using MongoDB.Bson;
using MongoDB.Bson.DefaultSerializer;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace MongoDB.BsonUnitTests.DefaultSerializer {
    [TestFixture]
    public class AnimalHierarchyWithAttributesTests {
        [BsonDiscriminator(RootClass = true)]
        [BsonKnownTypes(typeof(Bear), typeof(Cat))]
        public abstract class Animal {
            public ObjectId Id { get; set; }
            public int Age { get; set; }
            public string Name { get; set; }
        }

        public class Bear : Animal {
        }

        [BsonKnownTypes(typeof(Tiger), typeof(Lion))]
        public abstract class Cat : Animal {
        }

        public class Tiger : Cat {
        }

        public class Lion : Cat {
        }

        static AnimalHierarchyWithAttributesTests() {
            BsonClassMap.RegisterClassMap<Animal>();
        }

        [Test]
        public void TestDeserializeBear() {
            var document = new BsonDocument {
                { "_t", new BsonArray { "Animal", "Bear" } },
                { "Id", ObjectId.Empty },
                { "Age", 123 },
                { "Name", "Panda Bear" }
            };

            var bson = document.ToBson();
            var rehydrated = (Bear) BsonSerializer.DeserializeDocument<Animal>(bson);
            Assert.IsInstanceOf<Bear>(rehydrated);

            var json = rehydrated.ToJson<Animal>();
            var expected = "{ '_t' : ['Animal', 'Bear'], 'Id' : { '$oid' : '000000000000000000000000' }, 'Age' : 123, 'Name' : 'Panda Bear' }".Replace("'", "\"");
            Assert.AreEqual(expected, json);
            Assert.IsTrue(bson.SequenceEqual(rehydrated.ToBson<Animal>()));
        }

        [Test]
        public void TestDeserializeTiger() {
            var document = new BsonDocument {
                { "_t", new BsonArray { "Animal", "Cat", "Tiger" } },
                { "Id", ObjectId.Empty },
                { "Age", 234 },
                { "Name", "Striped Tiger" }
            };

            var bson = document.ToBson();
            var rehydrated = (Tiger) BsonSerializer.DeserializeDocument<Animal>(bson);
            Assert.IsInstanceOf<Tiger>(rehydrated);

            var json = rehydrated.ToJson<Animal>();
            var expected = "{ '_t' : ['Animal', 'Cat', 'Tiger'], 'Id' : { '$oid' : '000000000000000000000000' }, 'Age' : 234, 'Name' : 'Striped Tiger' }".Replace("'", "\"");
            Assert.AreEqual(expected, json);
            Assert.IsTrue(bson.SequenceEqual(rehydrated.ToBson<Animal>()));
        }

        [Test]
        public void TestDeserializeLion() {
            var document = new BsonDocument {
                { "_t", new BsonArray { "Animal", "Cat", "Lion" } },
                { "Id", ObjectId.Empty },
                { "Age", 234 },
                { "Name", "King Lion" }
            };

            var bson = document.ToBson();
            var rehydrated = (Lion) BsonSerializer.DeserializeDocument<Animal>(bson);
            Assert.IsInstanceOf<Lion>(rehydrated);

            var json = rehydrated.ToJson<Animal>();
            var expected = "{ '_t' : ['Animal', 'Cat', 'Lion'], 'Id' : { '$oid' : '000000000000000000000000' }, 'Age' : 234, 'Name' : 'King Lion' }".Replace("'", "\"");
            Assert.AreEqual(expected, json);
            Assert.IsTrue(bson.SequenceEqual(rehydrated.ToBson<Animal>()));
        }
    }
}