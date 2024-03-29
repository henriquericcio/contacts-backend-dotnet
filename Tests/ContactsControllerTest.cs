using ContactsBackendDotnet.Controllers;
using ContactsBackendDotnet.Infrastructure;
using ContactsBackendDotnet.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ContactsBackendDotnet.Tests;

public class ContactsControllerTest
{
    [Fact]
    public async Task Can_get_all_contactsAsync()
    {
        var context = await GetContactContext();

        context.AddRange(
            new Contact { FirstName = "Albert", LastName = "Einstein", PhoneNumber = "2222-1111" },
            new Contact { FirstName = "Marie", LastName = "Curie", PhoneNumber = "1111-1111" });
        await context.SaveChangesAsync();

        var result = await new ContactsController(null, context).Get();
        var actual = result.Value as IList<Contact>;

        Assert.NotNull(actual);
        Assert.Equal(2, actual.Count);
        Assert.Equal("Albert", actual[0].FirstName);
        Assert.Equal("Marie", actual[1].FirstName);
    }

    [Fact]
    public async Task Can_get_one_contact()
    {
        var context = await GetContactContext();

        var contact = new Contact { FirstName = "Albert", LastName = "Einstein", PhoneNumber = "2222-1111" };
        var expected = contact.Clone();
        context.Add(contact);
        await context.SaveChangesAsync();

        var result = await new ContactsController(null, context).Get(expected.Id);
        var actual = result.Value;

        Assert.NotNull(actual);
        Assert.Equal(expected.FirstName, actual.FirstName);
        Assert.Equal(expected.LastName, actual.LastName);
        Assert.Equal(expected.PhoneNumber, actual.PhoneNumber);
    }


    [Fact]
    public async Task Can_delete_a_contact()
    {
        var context = await GetContactContext();

        var contact = new Contact { FirstName = "Albert", LastName = "Einstein", PhoneNumber = "2222-1111" };
        var expected = contact.Clone();
        await context.AddAsync(contact);
        await context.SaveChangesAsync();

        var result = await new ContactsController(null, context).Delete(expected.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(context.Contacts);
    }

    
    [Fact]
    public async Task Can_create_a_contact()
    {
        var context = await GetContactContext();

        var contact = new Contact { FirstName = "Albert", LastName = "Einstein", PhoneNumber = "2222-1111" };
        var expected = contact.Clone();

        var result = await new ContactsController(null, context).Post(contact);

        Assert.IsType<CreatedAtActionResult>(result);
        var actual = ((CreatedAtActionResult)result).Value as Contact;
        Assert.Equal(expected.FirstName, actual.FirstName);
        Assert.Equal(expected.LastName, actual.LastName);
        Assert.Equal(expected.PhoneNumber, actual.PhoneNumber);

        Assert.Single(context.Contacts, c => c.Id == expected.Id);
    }


    [Fact]
    public async Task Can_update_a_contact()
    {
        var context = await GetContactContext();

        var contact = new Contact { FirstName = "Albert", LastName = "Einstein", PhoneNumber = "2222-1111" };
        await context.AddAsync(contact);
        await context.SaveChangesAsync();

        var changedContact = contact.Clone();
        changedContact.FirstName = "Ulbert";
        changedContact.LastName = "Oinstein";
        changedContact.PhoneNumber = "3333-4444";

        var expected = changedContact.Clone();

        var result = await new ContactsController(null, context).Update(changedContact, changedContact.Id);
        Assert.IsType<NoContentResult>(result);
        Assert.Single(context.Contacts, c => c.Id == changedContact.Id);

        var actual = await context.Contacts.FindAsync(expected.Id);
        Assert.Equal(expected.FirstName, actual.FirstName);
        Assert.Equal(expected.LastName, actual.LastName);
        Assert.Equal(expected.PhoneNumber, actual.PhoneNumber);
    }

    private static Task<ContactContext> GetContactContext() =>
        new TestDbContextFactory().CreateContextAsync();
    
}