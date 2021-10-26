DO $$
BEGIN
    INSERT INTO feeshares.fee_payments ("PeriodFrom", "PeriodTo","ReferrerClientId", "Amount", "CalculationTimestamp", "PaymentTimestamp", "Status", "PaymentOperationId")
        SELECT '${PeriodFrom}', '${PeriodTo}', "ReferrerClientId", SUM("FeeShareAmountInUsd"), current_timestamp, NULL, 0, NULL 
        FROM feeshares.fee_shares 
        WHERE "PaymentTimestamp" >= '${PeriodFrom}' AND "PaymentTimestamp" <= '${PeriodTo}' 
        GROUP BY "ReferrerClientId"
    ON CONFLICT ("PeriodFrom","ReferrerClientId") DO UPDATE SET ("Amount", "CalculationTimestamp") = (excluded."Amount", excluded."CalculationTimestamp");

    INSERT INTO feeshares.share_statistics ("PeriodFrom", "PeriodTo", "Amount", "CalculationTimestamp")
        SELECT '${PeriodFrom}', '${PeriodTo}', SUM("Amount"), current_timestamp
        FROM feeshares.fee_payments
        WHERE feeshares.fee_payments."PeriodFrom" = '${PeriodFrom}' AND "PeriodTo" = '${PeriodTo}'
        GROUP BY "ReferrerClientId"
    ON CONFLICT ("PeriodFrom", "PeriodTo") DO UPDATE SET ("Amount", "CalculationTimestamp") = (excluded."Amount", excluded."CalculationTimestamp");
END $$;
