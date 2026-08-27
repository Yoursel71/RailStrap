import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";

const items = [
  {
    q: "Roblox won't launch, or the download fails",
    a: "Usually antivirus or firewall software blocking RailStrap from reaching Roblox's servers. Temporarily allow it through your firewall/antivirus and try again.",
  },
  {
    q: "RailStrap crashes or won't open",
    a: (
      <>
        Try reinstalling the latest release. If that doesn&apos;t help, delete{" "}
        <code className="rounded bg-white/10 px-1.5 py-0.5 font-mono text-[12.5px]">
          %LocalAppData%\RailStrap
        </code>{" "}
        to reset your configuration, then reinstall.
      </>
    ),
  },
  {
    q: "Windows SmartScreen is blocking the installer",
    a: 'Expected — RailStrap ships unsigned. Click "More info", then "Run anyway".',
  },
  {
    q: "Discord Rich Presence isn't showing up",
    a: "Make sure Activity Tracking and Discord Rich Presence are both enabled in Integrations settings, and Discord is running before you launch Roblox.",
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
        </a>{" "}
        — RailStrap&apos;s crash/error dialogs can generate a pre-filled report for you.
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
        <h2 className="mb-3 text-[28px] font-extrabold tracking-tight">Common issues</h2>
        <p className="mb-8 text-[15px] text-muted-foreground">
          Quick fixes for the most frequent problems.
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
