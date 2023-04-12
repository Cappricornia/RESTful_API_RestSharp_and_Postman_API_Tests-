using RestSharp;
using System.Net;
using System.Text.Json;

namespace RestSharpAPITests
{
    public class RestSharpAPI_Tests
    {
        private RestClient client;
        private const string baseUrl = "https://contactbook.cappricornia.repl.co/api";

        [SetUp]
        public void SetUp()
        {
            this.client = new RestClient(baseUrl);
        }

        [Test]
        public void Test_List_All_Contacts_Assert_First_Contact_Names()
        {


            var request = new RestRequest("contacts", Method.Get);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var contacts = JsonSerializer.Deserialize<List<Contact>>(response.Content);
            Assert.That(contacts[0].firstName, Is.EqualTo("Steve"));
            Assert.That(contacts[0].lastName, Is.EqualTo("Jobs"));

        }

        

        [Test]
        public void Test_Find_Contact_By_Valid_Keyword()
        {
   
            var request = new RestRequest("contacts/search/albert", Method.Get);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var contact = JsonSerializer.Deserialize<List<Contact>>(response.Content);

            Assert.That(contact[0].firstName, Is.EqualTo("Albert"));
            Assert.That(contact[0].lastName, Is.EqualTo("Einstein"));
            
            

        }


        [Test]
        public void Test_Find_Contact_By_Missing_RandomNum_By_Invalid_Keyword()
        {

            var request = new RestRequest("contacts/search/missing" + DateTime.Now.Ticks, Method.Get);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(response.Content, Is.EqualTo("[]"));

        }


        [Test]
        public void Test_Try_To_Create_Contact_With_Invalid_Data()
        {

            var request = new RestRequest("contacts", Method.Post);

            var bodyResp = new
            {
                lastName = "Abraham",
                email = "email@myemail.com",
                phone = "+ 1 234 454"

            };

            request.AddBody(bodyResp);

            var response = this.client.Execute(request);


            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Is.EqualTo("{\"errMsg\":\"First name cannot be empty!\"}"));

        }

        [Test]
        public void Test_Try_To_Create_Contact_With_Valid_Data()
        {
           

            var request = new RestRequest("contacts", Method.Post);

            var bodyResp = new
            {
                firstName = "Merry",
                lastName = "Abraham",
                email = "merry@myemail.com",
                phone = "+ 1 234 454 000",
                comments = "some contact comments"

            };

            request.AddBody(bodyResp);

            var response = this.client.Execute(request);


            var contactObj = JsonSerializer.Deserialize<ContactObj>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(contactObj.msg, Is.EqualTo("Contact added."));
            Assert.That(contactObj.contact.id, Is.GreaterThan(0));
            Assert.That(contactObj.contact.firstName, Is.EqualTo(bodyResp.firstName));
            Assert.That(contactObj.contact.lastName, Is.EqualTo(bodyResp.lastName));
            Assert.That(contactObj.contact.phone, Is.EqualTo(bodyResp.phone));
            Assert.That(contactObj.contact.email, Is.EqualTo(bodyResp.email));
            Assert.That(contactObj.contact.dateCreated, Is.Not.Empty);
            Assert.That(contactObj.contact.comments, Is.EqualTo(bodyResp.comments));
        }
    }
}