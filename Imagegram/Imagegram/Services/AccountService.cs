using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Imagegram.Data;
using Imagegram.Services.Contracts;
using Imagegram.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Services
{
    public class AccountService : IAccountService
    {
        private ImagegramContext _imagegramContext;

        public AccountService(ImagegramContext imagegramContext)
        {
            _imagegramContext = imagegramContext;
        }

        public async Task<Account> GetById(string id)
        {
            var accountEntity = await _imagegramContext.Accounts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);

            if (accountEntity == null)
                throw new Exception(ErrorCodes.ResourceNotFound);

            return new Account
            {
                Id = accountEntity.Id,
                Name = accountEntity.Name
            };
        }

        public async Task<Account> CreateAccount(Account account)
        {
            var existingAccount = await _imagegramContext.Accounts.AsNoTracking().AnyAsync(x => x.Name == account.Name);

            if (existingAccount)
                throw new Exception(ErrorCodes.AccountAlreadyExists);

            var accountEntity = new Data.Entities.Account()
            {
                Id = Guid.NewGuid().ToString(),
                Name = account.Name
            };
            await _imagegramContext.Accounts.AddAsync(accountEntity);
            await _imagegramContext.SaveChangesAsync();

            account.Id = accountEntity.Id;

            return account;
        }

        public async Task DeleteAccount(string id)
        {
            var accountEntity = await _imagegramContext.Accounts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            var imagesToRemove = new List<string>();

            if (accountEntity == null)
                throw new Exception(ErrorCodes.ResourceNotFound);

            var commentEntities =
                _imagegramContext.Comments.AsNoTracking().Where(x => x.Creator == id);

            var postEntities = _imagegramContext.Posts.AsNoTracking().Where(x => x.Creator == id);

            if (postEntities.Any())
            {
                foreach (var postEntity in postEntities)
                {
                    var imageGuid = postEntity.ImageUrl.Replace("/Image/", string.Empty);
                    imagesToRemove.Add($@"UploadedImages\{imageGuid}.jpg");
                }
            }

            _imagegramContext.Comments.RemoveRange(commentEntities);
            _imagegramContext.Posts.RemoveRange(postEntities);
            _imagegramContext.Accounts.Remove(accountEntity);

            await _imagegramContext.SaveChangesAsync();

            foreach (var imageToRemove in imagesToRemove)
            {
                File.Delete(imageToRemove);
            }
        }
    }
}