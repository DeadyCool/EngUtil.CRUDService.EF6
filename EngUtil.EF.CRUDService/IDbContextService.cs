// --------------------------------------------------------------------------------
// <copyright filename="IRepositoryDto.cs" date="20-06-2016">(c) 2016 All Rights Reserved</copyright>
// <author>Oliver Engels</author>
// --------------------------------------------------------------------------------

using System.Data.Entity;

namespace EngUtil.EF.CRUDService
{
    public interface IDbContextService
    {
        DbContext CreateContext();
    }
}
