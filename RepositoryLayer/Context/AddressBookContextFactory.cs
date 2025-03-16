using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RepositoryLayer.Context;

public class AddressBookContextFactory : IDesignTimeDbContextFactory<AddressBookContext>
{
    public AddressBookContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AddressBookContext>();
        optionsBuilder.UseSqlServer("Server=127.0.0.1,1433;Database=FinalAddressBook;User=sa;Password={password};");

        return new AddressBookContext(optionsBuilder.Options);
    }
}