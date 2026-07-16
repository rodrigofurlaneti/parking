namespace Parking.Domain.Entities;

using Parking.Domain.Common;

// Simplificacao atual: assume-se 1 tarifa ATIVA por filial (BranchId), sem historico de vigencia
// por data. Se a tarifa de uma filial mudar, a pratica esperada e desativar a tarifa anterior
// (IsActive = false) e criar uma nova. Nao ha suporte, por ora, para consultar "qual era a tarifa
// vigente em uma data passada" - toda cobranca usa sempre a tarifa ATIVA no momento da consulta.
public sealed class Tariff : AggregateRoot
{
    public long BranchId { get; private set; }
    public decimal FirstHourRate { get; private set; }
    public decimal AdditionalHourRate { get; private set; }
    public decimal? DailyMaxRate { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Tariff() { }

    private Tariff(
        long branchId,
        decimal firstHourRate,
        decimal additionalHourRate,
        decimal? dailyMaxRate) : base(0)
    {
        BranchId = branchId;
        FirstHourRate = firstHourRate;
        AdditionalHourRate = additionalHourRate;
        DailyMaxRate = dailyMaxRate;
    }

    public static Result<Tariff> Create(
        long branchId,
        decimal firstHourRate,
        decimal additionalHourRate,
        decimal? dailyMaxRate)
    {
        if (firstHourRate < 0)
            return Result.Failure<Tariff>(
                new Error("Tariff.InvalidFirstHourRate", "First hour rate cannot be negative."));

        if (additionalHourRate < 0)
            return Result.Failure<Tariff>(
                new Error("Tariff.InvalidAdditionalHourRate", "Additional hour rate cannot be negative."));

        if (dailyMaxRate is not null && dailyMaxRate <= 0)
            return Result.Failure<Tariff>(
                new Error("Tariff.InvalidDailyMaxRate", "Daily max rate must be greater than 0 when informed."));

        return Result.Success(new Tariff(branchId, firstHourRate, additionalHourRate, dailyMaxRate));
    }

    // Regra de cobranca:
    // - Ate 60 minutos (inclusive): cobra somente a primeira hora (FirstHourRate).
    // - Acima de 60 minutos: primeira hora + horas adicionais cobradas por fracao (arredondando
    //   sempre para cima - qualquer minuto que exceda uma hora completa conta como hora cheia).
    // - Se DailyMaxRate estiver definido e o valor calculado ultrapassar o teto, o resultado e
    //   capado no teto diario.
    public decimal CalculateAmount(int durationMinutes)
    {
        decimal amount;

        if (durationMinutes <= 60)
        {
            amount = FirstHourRate;
        }
        else
        {
            var additionalMinutes = durationMinutes - 60;
            var additionalHours = (int)Math.Ceiling(additionalMinutes / 60.0);
            amount = FirstHourRate + (additionalHours * AdditionalHourRate);
        }

        if (DailyMaxRate is not null && amount > DailyMaxRate.Value)
            amount = DailyMaxRate.Value;

        return amount;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
