DO $$
BEGIN
    INSERT INTO feeshares.fee_payments ("PeriodFrom", "PeriodTo","ReferrerClientId", "Amount", "CalculationTimestamp", "Status")
        SELECT '${PeriodFrom}', '${PeriodTo}', "ReferrerClientId", SUM("FeeShareAmountInTargetAsset"), current_timestamp,  0 
        FROM feeshares.fee_shares 
        WHERE "PaymentTimestamp" > '${PeriodFrom}' AND "PaymentTimestamp" <= '${PeriodTo}' 
        GROUP BY "ReferrerClientId", "FeeShareAsset"
    ON CONFLICT ("PeriodFrom","PeriodTo","ReferrerClientId", "AssetId") DO UPDATE SET ("Amount", "CalculationTimestamp") = (excluded."Amount", excluded."CalculationTimestamp");

    INSERT INTO feeshares.share_statistics ("PeriodFrom", "PeriodTo", "Amount", "CalculationTimestamp", "Status")
        SELECT '${PeriodFrom}', '${PeriodTo}', SUM("Amount"), current_timestamp, 0
        FROM feeshares.fee_payments
        WHERE feeshares.fee_payments."PeriodFrom" = '${PeriodFrom}' AND "PeriodTo" = '${PeriodTo}'
        GROUP BY "ReferrerClientId", "AssetId"
    ON CONFLICT ("PeriodFrom", "PeriodTo", "AssetId") DO UPDATE SET ("Amount", "CalculationTimestamp") = (excluded."Amount", excluded."CalculationTimestamp");
END $$;
