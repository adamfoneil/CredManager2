using System;
using System.Linq;
using CredManager2.Models;
using CredManager2.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing
{
    [TestClass]
    public class ImportFeature
    {
        [TestMethod]
        public void FindNewEntries()
        {
            var sourceEntries = new Entry[]
            {
                new Entry() { Name = "This", DateCreated = new DateTime(2019, 2, 1), DateModified = new DateTime(2019, 3, 1) },
                new Entry() { Name = "That", DateCreated = new DateTime(2019, 2, 1)},
                new Entry() { Name = "Other", DateCreated = new DateTime(2019, 2, 1)}
            };

            var destEntries = new Entry[]
            {
                new Entry() { Name = "This", DateCreated = new DateTime(2019, 2, 1)},                
                new Entry() { Name = "Other", DateCreated = new DateTime(2019, 2, 1)}
            };
            
            var newEntries = ImportCommand.GetNewEntries(sourceEntries, destEntries).ToArray();
            Assert.IsTrue(newEntries[0].Equals(new Entry() { Name = "That" }));
        }

        [TestMethod]
        public void FindUpdatedEntries()
        {
            var sourceEntries = new Entry[]
            {
                new Entry() { Name = "This", DateCreated = new DateTime(2019, 2, 1), DateModified = new DateTime(2019, 3, 1) },
                new Entry() { Name = "That", DateCreated = new DateTime(2019, 2, 1)},
                new Entry() { Name = "Other", DateCreated = new DateTime(2019, 2, 1)}
            };

            var destEntries = new Entry[]
            {
                new Entry() { Name = "This", DateCreated = new DateTime(2019, 2, 1)},
                new Entry() { Name = "Other", DateCreated = new DateTime(2019, 2, 1)}
            };

            var updatedEntries = ImportCommand.GetUpdatedEntries(sourceEntries, destEntries).ToArray();
            Assert.IsTrue(updatedEntries[0].Equals(new Entry() { Name = "This" }));
        }
    }
}
