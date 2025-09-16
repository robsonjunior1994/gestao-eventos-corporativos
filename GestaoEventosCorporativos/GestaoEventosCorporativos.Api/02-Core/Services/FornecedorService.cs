using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Services;
using GestaoEventosCorporativos.Api._02_Core.Shared;

namespace GestaoEventosCorporativos.Api._02_Core.Services
{
    public class FornecedorService : IFornecedorService
    {
        private readonly IFornecedorRepository _fornecedorRepository;

        public FornecedorService(IFornecedorRepository fornecedorRepository)
        {
            _fornecedorRepository = fornecedorRepository;
        }

        public async Task<Result<Fornecedor>> AddAsync(Fornecedor fornecedor)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fornecedor.NomeServico))
                    return Result<Fornecedor>.Failure("O nome do serviço é obrigatório.", ErrorCode.VALIDATION_ERROR);

                if (string.IsNullOrWhiteSpace(fornecedor.CNPJ))
                    return Result<Fornecedor>.Failure("O CNPJ é obrigatório.", ErrorCode.VALIDATION_ERROR);

                if (fornecedor.ValorBase <= 0)
                    return Result<Fornecedor>.Failure("O valor base deve ser maior que zero.", ErrorCode.VALIDATION_ERROR);

                var existing = await _fornecedorRepository.GetByCnpjAsync(fornecedor.CNPJ);
                if (existing != null)
                    return Result<Fornecedor>.Failure("Já existe um fornecedor com este CNPJ.", ErrorCode.RESOURCE_ALREADY_EXISTS);

                await _fornecedorRepository.AddAsync(fornecedor);
                return Result<Fornecedor>.Success(fornecedor);
            }
            catch (Exception)
            {
                return Result<Fornecedor>.Failure("Erro ao criar fornecedor.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var fornecedor = await _fornecedorRepository.GetByIdAsync(id);
                if (fornecedor == null)
                    return Result<bool>.Failure("Fornecedor não encontrado.", ErrorCode.NOT_FOUND);

                await _fornecedorRepository.DeleteAsync(fornecedor);
                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure("Erro ao excluir fornecedor.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<IEnumerable<Fornecedor>>> GetAllAsync()
        {
            try
            {
                var fornecedores = await _fornecedorRepository.GetAllAsync();
                return Result<IEnumerable<Fornecedor>>.Success(fornecedores);
            }
            catch (Exception)
            {
                return Result<IEnumerable<Fornecedor>>.Failure("Erro ao buscar fornecedores.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<Fornecedor>> GetByIdAsync(int id)
        {
            try
            {
                var fornecedor = await _fornecedorRepository.GetByIdAsync(id);
                if (fornecedor == null)
                    return Result<Fornecedor>.Failure("Fornecedor não encontrado.", ErrorCode.NOT_FOUND);

                return Result<Fornecedor>.Success(fornecedor);
            }
            catch (Exception)
            {
                return Result<Fornecedor>.Failure("Erro ao buscar fornecedor.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<Fornecedor>> UpdateAsync(Fornecedor fornecedor)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fornecedor.NomeServico))
                    return Result<Fornecedor>.Failure("O nome do serviço é obrigatório.", ErrorCode.VALIDATION_ERROR);

                if (string.IsNullOrWhiteSpace(fornecedor.CNPJ))
                    return Result<Fornecedor>.Failure("O CNPJ é obrigatório.", ErrorCode.VALIDATION_ERROR);

                if (fornecedor.ValorBase <= 0)
                    return Result<Fornecedor>.Failure("O valor base deve ser maior que zero.", ErrorCode.VALIDATION_ERROR);

                var existing = await _fornecedorRepository.GetByIdAsync(fornecedor.Id);
                if (existing == null)
                    return Result<Fornecedor>.Failure("Fornecedor não encontrado.", ErrorCode.NOT_FOUND);

                var fornecedorByCnpj = await _fornecedorRepository.GetByCnpjAsync(fornecedor.CNPJ);
                if (fornecedorByCnpj != null && fornecedorByCnpj.Id != fornecedor.Id)
                    return Result<Fornecedor>.Failure("Já existe outro fornecedor com este CNPJ.", ErrorCode.RESOURCE_ALREADY_EXISTS);

                await _fornecedorRepository.UpdateAsync(fornecedor);
                return Result<Fornecedor>.Success(fornecedor);
            }
            catch (Exception)
            {
                return Result<Fornecedor>.Failure("Erro ao atualizar fornecedor.", ErrorCode.DATABASE_ERROR);
            }
        }
    }
}
