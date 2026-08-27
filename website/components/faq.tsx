import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";

const items = [
  {
    q: "Roblox won't launch, or the download fails",
    a: "Allow RailStrap through your firewall or antivirus, then retry.",
  },
  {
    q: "RailStrap crashes or won't open",
    a: (
      <>
        Reinstall. Still broken? Reset{" "}
        <code className="rounded bg-white/10 px-1.5 py-0.5 font-mono text-[12.5px]">
          %LocalAppData%\RailStrap
        </code>, then retry.
      </>
    ),
  },
  {
    q: "Windows SmartScreen is blocking the installer",
    a: 'Expected. Choose "More info", then "Run anyway".',
  },
  {
    q: "Discord Rich Presence isn't showing up",
    a: "Enable both integration toggles. Open Discord before Roblox.",
  },
  {
    q: "Still stuck?",
    a: (
      <>
        Open an issue on{" "}
        <a
          href="https://github.com/Yoursel71/RailStrap/issues"
          target="_blank"
          rel="noopener noreferrer"
          className="underline underline-offset-2"
        >
          GitHub Issues
        </a>.
      </>
    ),
  },
];

export function FAQ() {
  return (
    <section id="help" className="py-24">
      <div className="container max-w-[760px]">
        <p className="mb-2.5 text-xs font-bold uppercase tracking-wider text-[hsl(217,100%,68%)]">
          Help
        </p>
        <h2 className="mb-3 text-[28px] font-extrabold tracking-tight">Quick fixes.</h2>
        <p className="mb-8 text-[15px] text-muted-foreground">
          Short answers. No hunting.
        </p>

        <Accordion type="single" collapsible className="flex flex-col gap-2.5">
          {items.map((item) => (
            <AccordionItem key={item.q} value={item.q}>
              <AccordionTrigger>{item.q}</AccordionTrigger>
              <AccordionContent>{item.a}</AccordionContent>
            </AccordionItem>
          ))}
        </Accordion>
      </div>
    </section>
  );
}
